using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Design;
using Newbe.Claptrap.Preview.Impl.Localization;
using static Newbe.Claptrap.Preview.Impl.Localization.LK.L0006ClaptrapFactory;

namespace Newbe.Claptrap.Preview.Impl
{
    public class ClaptrapFactory : IClaptrapFactory
    {
        private readonly ILogger<ClaptrapFactory> _logger;
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IL _l;

        public ClaptrapFactory(
            ILogger<ClaptrapFactory> logger,
            IClaptrapDesignStore claptrapDesignStore,
            ILifetimeScope lifetimeScope,
            IL l)
        {
            _logger = logger;
            _claptrapDesignStore = claptrapDesignStore;
            _lifetimeScope = lifetimeScope;
            _l = l;
        }

        public IClaptrap Create(IClaptrapIdentity identity)
        {
            try
            {
                return CreateCore();
            }
            catch (Exception e)
            {
                _logger.LogError(e, _l[L001FailedToCreate], identity);
                throw;
            }

            IClaptrap CreateCore()
            {
                var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
                var actorScope = _lifetimeScope.BeginLifetimeScope(builder =>
                {
                    builder.Register(context => identity)
                        .AsSelf()
                        .SingleInstance();
                    builder.RegisterModule(new ClaptrapSharedModule(claptrapDesign, identity));
                    var masterDesign = claptrapDesign.ClaptrapMasterDesign;
                    if (masterDesign != null)
                    {
                        _logger.LogDebug(_l[L002MasterFound], masterDesign.Identity.TypeCode);
                        var moduleIdentity = new ClaptrapIdentity(identity.Id,
                            masterDesign.Identity.TypeCode);
                        builder.RegisterModule(new ClaptrapMinionModule(masterDesign,
                            moduleIdentity));
                    }
                    else
                    {
                        _logger.LogDebug(_l[L003MasterFound], identity.TypeCode);
                        builder.RegisterModule(new ClaptrapMasterModule(claptrapDesign, identity));
                    }
                });
                var actor = actorScope.Resolve<ClaptrapActor>();
                return actor;
            }
        }

        private class ClaptrapSharedModule : Module
        {
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
                RegisterComponent<IStateSaver>(_claptrapDesign.StateSaverFactoryType);
                RegisterComponent<IStateLoader>(_claptrapDesign.StateLoaderFactoryType);
                RegisterComponent<IEventHandlerFactory>(_claptrapDesign.EventHandlerFactoryFactoryType);
                RegisterComponent<IStateHolder>(_claptrapDesign.StateHolderFactoryType);
                builder.Register(t => t.Resolve(_claptrapDesign.InitialStateDataFactoryType))
                    .As<IInitialStateDataFactory>()
                    .SingleInstance();
                builder.RegisterInstance(_claptrapDesign.StateOptions);

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

        private class ClaptrapMinionModule : Module
        {
            private readonly IClaptrapDesign _masterDesign;
            private readonly ClaptrapIdentity _masterIdentity;

            public ClaptrapMinionModule(IClaptrapDesign masterDesign,
                ClaptrapIdentity masterIdentity)
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

        private class ClaptrapMasterModule : Module
        {
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
                RegisterComponent<IEventLoader>(_claptrapDesign.EventLoaderFactoryType);
                RegisterComponent<IEventSaver>(_claptrapDesign.EventSaverFactoryType);
                builder.RegisterType<MasterEventHandlerFLow>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
                builder.RegisterType<EventHandledNotificationFlow>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
                builder.RegisterModule(new EventCenterNotifierModule(_identity));
          
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