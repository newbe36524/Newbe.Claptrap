using Autofac;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
{
    public class SQLiteStorageSharedModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "SQLite storage shared module";
        public string Description { get; } = "Module for support event store and state store by using SQLite";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .SingleInstance();
            
            builder.RegisterBuildCallback(container =>
            {
                var cache = container.Resolve<ISqlTemplateCache>();
                SQLiteEventEntitySaver.RegisterParameters(cache, 1000);
            });
        }
    }
}