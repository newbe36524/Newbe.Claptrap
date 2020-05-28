using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneTypeOneTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.Relational.StateStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.Relational.Module
{
    public class ClaptrapRelationalStorageProviderModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Claptrap relational database storage provider model";
        public string Description { get; } = "Module for sharing components in relation database storage providers";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SqlTemplateCache>()
                .As<ISqlTemplateCache>()
                .SingleInstance();

            builder.RegisterType<SharedTableEventEntityMapper>()
                .As<IEventEntityMapper<SharedTableEventEntity>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OneTypeOneTableEventEntityMapper>()
                .As<IEventEntityMapper<OneTypeOneTableEventEntity>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OneIdentityOneTableEventEntityMapper>()
                .As<IEventEntityMapper<OneIdentityOneTableEventEntity>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OneIdentityOneTableStateEntityMapper>()
                .As<IStateEntityMapper<OneIdentityOneTableStateEntity>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<SharedTableStateEntityMapper>()
                .As<IStateEntityMapper<SharedTableStateEntity>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SQLiteStoreFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}