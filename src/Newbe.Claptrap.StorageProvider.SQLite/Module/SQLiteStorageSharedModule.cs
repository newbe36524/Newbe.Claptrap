using Autofac;
using Newbe.Claptrap.StorageProvider.Relational;

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

            builder.RegisterBuildCallback(container =>
            {
                var cache = container.Resolve<ISqlTemplateCache>();
            });
        }
    }
}