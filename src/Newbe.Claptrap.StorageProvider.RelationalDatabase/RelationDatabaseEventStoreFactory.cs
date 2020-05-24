using System;
using Autofac;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.OneTypeOneTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.Module;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.Options;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public class RelationDatabaseEventStoreFactory :
        IClaptrapComponentFactory<IEventSaver>,
        IClaptrapComponentFactory<IEventLoader>
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public RelationDatabaseEventStoreFactory(
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
            if (saverOptions is IRelationDatabaseEventSaverOptions relationDatabaseEventSaverOptions)
            {
                var scope = _lifetimeScope.BeginLifetimeScope(builder =>
                {
                    if (saverOptions is IBatchEventSaverOptions batchEventSaverOptions)
                    {
                        builder.RegisterInstance(batchEventSaverOptions)
                            .SingleInstance();
                    }

                    if (saverOptions is IAutoMigrationOptions autoMigrationOptions
                        && autoMigrationOptions.Enabled)
                    {
                        builder.RegisterType<AutoMigrationEventSaver>()
                            .AsSelf()
                            .InstancePerLifetimeScope();
                        builder.RegisterDecorator<IEventSaver>((context, ps, inner) => context
                            .Resolve<AutoMigrationEventSaver.Factory>()
                            .Invoke(inner), context => true);
                    }
                });
                var eventSaver = relationDatabaseEventSaverOptions.EventStoreStrategy switch
                {
                    EventStoreStrategy.SharedTable => scope.Resolve(
                        typeof(RelationDatabaseEventSaver<SharedTableEventEntity>)),
                    EventStoreStrategy.OneTypeOneTable => scope.Resolve(
                        typeof(RelationDatabaseEventSaver<OneTypeOneTableEventEntity>)),
                    EventStoreStrategy.OneIdentityOneTable => scope.Resolve(
                        typeof(RelationDatabaseEventSaver<OneIdentityOneTableEventEntity>)),
                    _ => throw new ArgumentOutOfRangeException()
                };

                return (IEventSaver) eventSaver;
            }

            // TODO 
            throw new NotSupportedException();
        }

        IEventLoader IClaptrapComponentFactory<IEventLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            throw new System.NotImplementedException();
        }
    }
}