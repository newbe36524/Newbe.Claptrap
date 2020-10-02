using Autofac;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.StateStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
{
    public class SQLiteStorageSharedModule : Autofac.Module, IClaptrapAppModule
    {
        public string Name { get; } = "SQLite storage shared module";
        public string Description { get; } = "Module for support event store and state store by using SQLite";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SQLiteDbFactory>()
                .As<ISQLiteDbFactory>()
                .SingleInstance();
            builder.RegisterType<SQLiteAdoNetCache>()
                .As<ISQLiteAdoNetCache>()
                .SingleInstance();

            builder.RegisterBuildCallback(container =>
            {
                var cache = container.Resolve<ISqlTemplateCache>();
                SQLiteStateEntitySaver.RegisterParameters(cache, 1000);
                var adoCache = container.Resolve<ISQLiteAdoNetCache>();
                SQLiteEventEntitySaver.RegisterParameters(adoCache, 1000);
            });
        }
    }
}