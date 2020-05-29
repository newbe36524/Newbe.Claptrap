using Autofac;
using Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.Module
{
    public class MySqlStorageModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "MySql storage module";
        public string Description { get; } = "Module for support event store and state store by using MySql";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .SingleInstance();

            builder.RegisterType<SharedTableEventBatchSaver>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<SharedTableEventBatchSaverFactory>()
                .As<ISharedTableEventBatchSaverFactory>()
                .SingleInstance();

            builder.RegisterType<DbUpMysqlMigration>()
                .AsSelf()
                .InstancePerDependency();
        }
    }
}