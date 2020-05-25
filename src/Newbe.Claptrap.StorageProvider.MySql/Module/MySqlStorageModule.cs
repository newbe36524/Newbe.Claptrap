using Autofac;
using Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.Module
{
    public class MySqlStorageModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "MySql storage module";
        public string Description { get; } = "Module for support event store and state store by using MySql";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register(t => t.Resolve<IEventStoreMigrationFactory>()
                    .CreateEventLoaderMigration(t.Resolve<IClaptrapIdentity>()))
                .As<IEventLoaderMigration>();
            builder.Register(t => t.Resolve<IEventStoreMigrationFactory>()
                    .CreateEventSaverMigration(t.Resolve<IClaptrapIdentity>()))
                .As<IEventSaverMigration>();

            builder.RegisterType<MySqlSqlCacheHelper>()
                .As<IMySqlSqlCacheHelper>()
                .SingleInstance();

            builder.RegisterType<MySqlSharedTableEventEntityLoader>()
                .As<IEventEntityLoader<SharedTableEventEntity>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MySqlSharedTableEventEntitySaver>()
                .As<IEventEntitySaver<SharedTableEventEntity>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SharedTableEventStoreDbUpMysqlMigration>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<EventStoreMigrationFactory>()
                .As<IEventStoreMigrationFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterBuildCallback(scope =>
            {
                var cacheHelper = scope.Resolve<IMySqlSqlCacheHelper>();
                cacheHelper.Init();
            });
        }
    }
}