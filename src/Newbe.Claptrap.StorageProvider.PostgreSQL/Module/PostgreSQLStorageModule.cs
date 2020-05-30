using Autofac;
using Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Module
{
    public class PostgreSQLStorageModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "PostgreSQL storage module";
        public string Description { get; } = "Module for support event store and state store by using PostgreSQL";

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
        }
    }
}