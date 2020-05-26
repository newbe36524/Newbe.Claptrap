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
                            .Invoke(inner), context => true);
                    }
                });
                var eventSaver = relationalEventSaverOptions.EventStoreStrategy switch
                {
                    EventStoreStrategy.SharedTable => scope.Resolve(
                        typeof(RelationalEventSaver<SharedTableEventEntity>)),
                    EventStoreStrategy.OneTypeOneTable => scope.Resolve(
                        typeof(RelationalEventSaver<OneTypeOneTableEventEntity>)),
                    EventStoreStrategy.OneIdentityOneTable => scope.Resolve(
                        typeof(RelationalEventSaver<OneIdentityOneTableEventEntity>)),
                    _ => throw new ArgumentOutOfRangeException()
                };

                return (IEventSaver) eventSaver;
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
                            .Invoke(inner), context => true);
                    }
                });
                var eventLoader = relationalEventLoaderOptions.EventStoreStrategy switch
                {
                    EventStoreStrategy.SharedTable => scope.Resolve(
                        typeof(RelationalEventLoader<SharedTableEventEntity>)),
                    EventStoreStrategy.OneTypeOneTable => scope.Resolve(
                        typeof(RelationalEventLoader<OneTypeOneTableEventEntity>)),
                    EventStoreStrategy.OneIdentityOneTable => scope.Resolve(
                        typeof(RelationalEventLoader<OneIdentityOneTableEventEntity>)),
                    _ => throw new ArgumentOutOfRangeException()
                };

                return (IEventLoader) eventLoader;
            }

            throw new System.NotImplementedException();
        }
    }
}