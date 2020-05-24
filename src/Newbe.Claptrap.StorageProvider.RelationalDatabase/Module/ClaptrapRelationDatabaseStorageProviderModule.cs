using Autofac;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.OneTypeOneTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.Module
{
    public class ClaptrapRelationDatabaseStorageProviderModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Claptrap relation database storage provider model";
        public string Description { get; } = "Module for sharing components in relation database storage providers";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SqlCache>()
                .As<ISqlCache>()
                .SingleInstance();

            builder.RegisterType<RelationDatabaseEventLoader<SharedTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<RelationDatabaseEventLoader<OneTypeOneTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<RelationDatabaseEventLoader<OneIdentityOneTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RelationDatabaseEventSaver<SharedTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<RelationDatabaseEventSaver<OneTypeOneTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<RelationDatabaseEventSaver<OneIdentityOneTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<SharedTableEventEntityMapper>()
                .As<IEventEntityMapper<SharedTableEventEntity>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OneTypeOneTableEventEntityMapper>()
                .As<IEventEntityMapper<OneTypeOneTableEventEntity>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OneIdentityOneTableEventEntityMapper>()
                .As<IEventEntityMapper<OneIdentityOneTableEventEntity>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RelationDatabaseEventStoreFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}