using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneTypeOneTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.Relational.Module
{
    public class ClaptrapRelationalStorageProviderModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Claptrap relational database storage provider model";
        public string Description { get; } = "Module for sharing components in relation database storage providers";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SqlCache>()
                .As<ISqlCache>()
                .SingleInstance();

            builder.RegisterType<RelationalEventLoader<SharedTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<RelationalEventLoader<OneTypeOneTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<RelationalEventLoader<OneIdentityOneTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<RelationalEventSaver<SharedTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<RelationalEventSaver<OneTypeOneTableEventEntity>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<RelationalEventSaver<OneIdentityOneTableEventEntity>>()
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

            builder.RegisterType<RelationalEventStoreFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}