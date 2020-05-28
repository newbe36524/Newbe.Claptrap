using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdentityOneTable;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
{
    public class SQLiteStorageModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "SQLite storage module";
        public string Description { get; } = "Module for support event store and state store by using SQLite";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SQLiteSqlCache>()
                .As<ISQLiteSqlCache>()
                .SingleInstance();

            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .SingleInstance();

            builder.RegisterType<SQLieOneIdentityOneTableEventEntitySaver>()
                .As<IEventEntitySaver<OneIdentityOneTableEventEntity>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<SQLiteOneIdentityOneTableEventEntityLoader>()
                .As<IEventEntityLoader<OneIdentityOneTableEventEntity>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SQLieOneIdentityOneTableStateEntitySaver>()
                .As<IStateEntitySaver<OneIdentityOneTableStateEntity>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<SQLiteOneIdentityOneTableStateEntityLoader>()
                .As<IStateEntityLoader<OneIdentityOneTableStateEntity>>()
                .InstancePerLifetimeScope();


            builder.RegisterBuildCallback(scope =>
            {
                var sqLiteSqlCache = scope.Resolve<ISQLiteSqlCache>();
                sqLiteSqlCache.Init();
            });
        }
    }
}