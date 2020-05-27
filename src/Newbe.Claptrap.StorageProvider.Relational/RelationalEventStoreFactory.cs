using System;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneTypeOneTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class RelationalEventStoreFactory :
        IClaptrapComponentFactory<IEventSaver>,
        IClaptrapComponentFactory<IEventLoader>
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public RelationalEventStoreFactory(
            ILifetimeScope lifetimeScope,
            IClaptrapDesignStore claptrapDesignStore)
        {
            _lifetimeScope = lifetimeScope;
            _claptrapDesignStore = claptrapDesignStore;
        }

        IEventSaver IClaptrapComponentFactory<IEventSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(claptrapIdentity);
            var saverOptions = claptrapDesign.StorageProviderOptions.EventSaverOptions;
            if (saverOptions is IRelationalEventSaverOptions relationalEventSaverOptions)
            {
                var scope = _lifetimeScope.BeginLifetimeScope(builder =>
                {
                    if (saverOptions is IBatchEventSaverOptions batchEventSaverOptions)
                    {
                        builder.RegisterInstance(batchEventSaverOptions)
                            .SingleInstance();
                    }

                    if (saverOptions is IAutoMigrationOptions autoMigrationOptions
                        && autoMigrationOptions.IsAutoMigrationEnabled)
                    {
                        builder.RegisterType<AutoMigrationEventSaver>()
                            .AsSelf()
                            .InstancePerLifetimeScope();
                        builder.RegisterDecorator<IEventSaver>((context, ps, inner) => context
                            .Resolve<AutoMigrationEventSaver.Factory>()
                            .Invoke(inner));
                    }

                    switch (relationalEventSaverOptions.EventStoreStrategy)
                    {
                        case EventStoreStrategy.SharedTable:
                            builder.RegisterType<RelationalEventSaver<SharedTableEventEntity>>()
                                .As<IEventSaver>()
                                .InstancePerLifetimeScope();
                            break;
                        case EventStoreStrategy.OneTypeOneTable:
                            builder.RegisterType<RelationalEventSaver<OneTypeOneTableEventEntity>>()
                                .As<IEventSaver>()
                                .InstancePerLifetimeScope();
                            break;
                        case EventStoreStrategy.OneIdentityOneTable:
                            builder.RegisterType<RelationalEventSaver<OneIdentityOneTableEventEntity>>()
                                .As<IEventSaver>()
                                .InstancePerLifetimeScope();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
                var eventSaver = scope.Resolve<IEventSaver>();
                return eventSaver;
            }

            // TODO 
            throw new NotSupportedException();
        }

        IEventLoader IClaptrapComponentFactory<IEventLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(claptrapIdentity);
            var loaderOptions = claptrapDesign.StorageProviderOptions.EventLoaderOptions;
            if (loaderOptions is IRelationalEventLoaderOptions relationalEventLoaderOptions)
            {
                var scope = _lifetimeScope.BeginLifetimeScope(builder =>
                {
                    if (loaderOptions is IAutoMigrationOptions autoMigrationOptions
                        && autoMigrationOptions.IsAutoMigrationEnabled)
                    {
                        builder.RegisterType<AutoMigrationEventLoader>()
                            .AsSelf()
                            .InstancePerLifetimeScope();
                        builder.RegisterDecorator<IEventLoader>((context, ps, inner) => context
                            .Resolve<AutoMigrationEventLoader.Factory>()
                            .Invoke(inner));
                    }

                    switch (relationalEventLoaderOptions.EventStoreStrategy)
                    {
                        case EventStoreStrategy.SharedTable:
                            builder.RegisterType<RelationalEventLoader<SharedTableEventEntity>>()
                                .As<IEventLoader>()
                                .InstancePerLifetimeScope();
                            break;
                        case EventStoreStrategy.OneTypeOneTable:
                            builder.RegisterType<RelationalEventLoader<OneTypeOneTableEventEntity>>()
                                .As<IEventLoader>()
                                .InstancePerLifetimeScope();
                            break;
                        case EventStoreStrategy.OneIdentityOneTable:
                            builder.RegisterType<RelationalEventLoader<OneIdentityOneTableEventEntity>>()
                                .As<IEventLoader>()
                                .InstancePerLifetimeScope();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
                var eventLoader = scope.Resolve<IEventLoader>();
                return eventLoader;
            }

            throw new System.NotImplementedException();
        }
    }
}