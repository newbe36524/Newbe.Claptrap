using System;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl
{
    public class ClaptrapFactory : IClaptrapFactory
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly ILifetimeScope _lifetimeScope;

        public ClaptrapFactory(
            IClaptrapDesignStore claptrapDesignStore,
            ILifetimeScope lifetimeScope)
        {
            _claptrapDesignStore = claptrapDesignStore;
            _lifetimeScope = lifetimeScope;
        }

        public IClaptrap Create(IClaptrapIdentity identity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
            var actorScope = _lifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.Register(context => identity)
                    .AsSelf()
                    .SingleInstance();
                builder.RegisterModule(new ClaptrapDesignModule(claptrapDesign));
            });
            var actor = actorScope.Resolve<ClaptrapActor>();
            return actor;
        }

        private class ClaptrapDesignModule : Module
        {
            private readonly IClaptrapDesign _claptrapDesign;

            public ClaptrapDesignModule(
                IClaptrapDesign claptrapDesign)
            {
                _claptrapDesign = claptrapDesign;
            }

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                RegisterComponent<IStateSaver>(_claptrapDesign.StateSaverFactoryType);
                RegisterComponent<IStateLoader>(_claptrapDesign.StateLoaderFactoryType);

                RegisterComponent<IEventLoader>(_claptrapDesign.EventLoaderFactoryType);
                RegisterComponent<IEventSaver>(_claptrapDesign.EventSaverFactoryType);
                RegisterComponent<IEventHandlerFactory>(_claptrapDesign.EventHandlerFactoryFactoryType);
                RegisterComponent<IStateHolder>(_claptrapDesign.StateHolderFactoryType);
                builder.Register(t => t.Resolve(_claptrapDesign.InitialStateDataFactoryType))
                    .As<IInitialStateDataFactory>()
                    .SingleInstance();
                builder.RegisterInstance(_claptrapDesign.StateSavingOptions);

                void RegisterComponent<TComponent>(Type factoryType)
                    where TComponent : class, IClaptrapComponent
                {
                    builder.Register(t =>
                            ((IClaptrapComponentFactory<TComponent>) t.Resolve(factoryType))
                            .Create(
                                t.Resolve<IClaptrapIdentity>()))
                        .As<TComponent>()
                        .SingleInstance();
                }
            }
        }
    }
}