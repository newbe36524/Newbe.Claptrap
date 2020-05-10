using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autofac;
using Autofac.Core;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Core.Impl;
using Newbe.Claptrap.Design;

namespace Newbe.Claptrap.Modules
{
    public class ClaptrapModuleProvider : IClaptrapModuleProvider
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public ClaptrapModuleProvider(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public IEnumerable<IClaptrapSharedModule> GetClaptrapSharedModules(IClaptrapIdentity identity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
            var re = new ClaptrapSharedModule(claptrapDesign, identity);
            yield return re;
        }

        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
            var re = new ClaptrapMasterModule(claptrapDesign, identity);
            yield return re;
        }

        public IEnumerable<IClaptrapMinionModule> GetClaptrapMinionModules(IClaptrapIdentity identity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
            var moduleIdentity = new ClaptrapIdentity(identity.Id,
                identity.TypeCode);
            var re = new ClaptrapMinionModule(claptrapDesign, moduleIdentity);
            yield return re;
        }


        private class ClaptrapSharedModule : Module, IClaptrapSharedModule
        {
            public string Name { get; } = "Claptrap shared module";
            public string Description { get; } = "Module for claptrap and minion shared components";
            private readonly IClaptrapDesign _claptrapDesign;
            private readonly IClaptrapIdentity _identity;

            public ClaptrapSharedModule(
                IClaptrapDesign claptrapDesign,
                IClaptrapIdentity identity)
            {
                _claptrapDesign = claptrapDesign;
                _identity = identity;
            }

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);

                builder.RegisterType<ClaptrapActor>()
                    .As<IClaptrap>()
                    .SingleInstance();
                builder.RegisterDecorator<AopClaptrap, IClaptrap>(context => true);

                RegisterComponent<IStateSaver>(_claptrapDesign.StateSaverFactoryType);
                RegisterComponent<IStateLoader>(_claptrapDesign.StateLoaderFactoryType);
                RegisterComponent<IEventHandlerFactory>(_claptrapDesign.EventHandlerFactoryFactoryType);
                RegisterComponent<IStateHolder>(_claptrapDesign.StateHolderFactoryType);
                builder.Register(t => t.Resolve(_claptrapDesign.InitialStateDataFactoryType))
                    .As<IInitialStateDataFactory>()
                    .SingleInstance();
                builder.RegisterInstance(_claptrapDesign.ClaptrapOptions);
                builder.RegisterInstance(_claptrapDesign.ClaptrapOptions.EventLoadingOptions);
                builder.RegisterInstance(_claptrapDesign.ClaptrapOptions.StateRecoveryOptions);
                builder.RegisterInstance(_claptrapDesign.ClaptrapOptions.StateSavingOptions);
                builder.RegisterInstance(_claptrapDesign.ClaptrapOptions.MinionActivationOptions);

                builder.RegisterType<StateAccessor>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
                builder.RegisterType<StateRestorer>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
                builder.RegisterType<StateSavingFlow>()
                    .AsImplementedInterfaces()
                    .SingleInstance();

                void RegisterComponent<TComponent>(Type factoryType)
                    where TComponent : class, IClaptrapComponent
                {
                    builder.Register(t =>
                            ((IClaptrapComponentFactory<TComponent>) t.Resolve(factoryType))
                            .Create(_identity))
                        .As<TComponent>()
                        .SingleInstance();
                }
            }
        }

        private class ClaptrapMinionModule : Module, IClaptrapMinionModule
        {
            public string Name { get; } = "Claptrap minion module";
            public string Description { get; } = "Module for minion";
            private readonly IClaptrapDesign _masterDesign;
            private readonly IClaptrapIdentity _masterIdentity;

            public ClaptrapMinionModule(IClaptrapDesign masterDesign,
                IClaptrapIdentity masterIdentity)
            {
                _masterDesign = masterDesign;
                _masterIdentity = masterIdentity;
            }

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);

                RegisterComponent<IEventLoader>(_masterDesign.EventLoaderFactoryType);

                builder.RegisterType<MinionEventHandlerFLow>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
                builder.RegisterType<EmptyEventHandledNotificationFlow>()
                    .AsImplementedInterfaces()
                    .SingleInstance();

                void RegisterComponent<TComponent>(Type factoryType)
                    where TComponent : class, IClaptrapComponent
                {
                    builder.Register(t =>
                            ((IClaptrapComponentFactory<TComponent>) t.Resolve(factoryType))
                            .Create(_masterIdentity))
                        .As<TComponent>()
                        .SingleInstance();
                }
            }
        }

        private class ClaptrapMasterModule : Module, IClaptrapMasterModule
        {
            public string Name { get; } = "Claptrap master module";
            public string Description { get; } = "Module for master claptrap";
            private readonly IClaptrapDesign _claptrapDesign;
            private readonly IClaptrapIdentity _identity;

            public ClaptrapMasterModule(
                IClaptrapDesign claptrapDesign,
                IClaptrapIdentity identity)
            {
                _claptrapDesign = claptrapDesign;
                _identity = identity;
            }

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);

                var minionOptions = _claptrapDesign.ClaptrapOptions.MinionActivationOptions;
                Debug.Assert(minionOptions != null, nameof(minionOptions) + " != null");
                if (minionOptions.ActivateMinionsAtMasterStart)
                {
                    builder.RegisterType<WakeMinionClaptrapLifetimeInterceptor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                }

                RegisterComponent<IEventLoader>(_claptrapDesign.EventLoaderFactoryType);
                RegisterComponent<IEventSaver>(_claptrapDesign.EventSaverFactoryType);
                builder.RegisterType<MasterEventHandlerFLow>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
                builder.RegisterType<EventHandledNotificationFlow>()
                    .AsImplementedInterfaces()
                    .SingleInstance();

                
                // TODO move
                builder.RegisterType<EventCenterEventNotifier>()
                    .AsSelf()
                    .InstancePerDependency();
                builder.RegisterType<EventCenterEventHandledNotifierFactory>()
                    .AsSelf()
                    .SingleInstance();
                RegisterComponent<IEventNotifier>(typeof(EventCenterEventHandledNotifierFactory));

                void RegisterComponent<TComponent>(Type factoryType)
                    where TComponent : class, IClaptrapComponent
                {
                    builder.Register(t =>
                            ((IClaptrapComponentFactory<TComponent>) t.Resolve(factoryType))
                            .Create(_identity))
                        .As<TComponent>()
                        .SingleInstance();
                }
            }
        }
    }
}