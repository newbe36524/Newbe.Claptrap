using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl
{
    public class ClaptrapFactory : IClaptrapFactory
    {
        private readonly ILogger<ClaptrapFactory> _logger;
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly ILifetimeScope _lifetimeScope;

        public ClaptrapFactory(
            ILogger<ClaptrapFactory> logger,
            IClaptrapDesignStore claptrapDesignStore,
            ILifetimeScope lifetimeScope)
        {
            _logger = logger;
            _claptrapDesignStore = claptrapDesignStore;
            _lifetimeScope = lifetimeScope;
        }

        public IClaptrap Create(IClaptrapIdentity identity)
        {
            try
            {
                return CreateCore();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to create a claptrap. {identity}", identity);
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
                    builder.RegisterModule(new ClaptrapDesignModule(claptrapDesign));
                });
                var actor = actorScope.Resolve<ClaptrapActor>();
                return actor;
            }
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
                // TODO move notifier
                builder.RegisterType<EmptyEventHandledNotifier>()
                    .AsSelf()
                    .SingleInstance();
                builder.RegisterType<EmptyEventHandledNotifierFactory>()
                    .AsSelf()
                    .SingleInstance();
                RegisterComponent<IEventHandledNotifier>(typeof(EmptyEventHandledNotifierFactory));
                builder.Register(t => t.Resolve(_claptrapDesign.InitialStateDataFactoryType))
                    .As<IInitialStateDataFactory>()
                    .SingleInstance();
                builder.RegisterInstance(_claptrapDesign.StateOptions);

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