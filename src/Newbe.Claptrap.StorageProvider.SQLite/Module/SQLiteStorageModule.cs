using Autofac;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
{
    public class SQLiteStorageModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "SQLite storage module";
        public string Description { get; } = "Module for support event store and state store by using SQLite";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SQLiteEventStore>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<SQLiteStateStore>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<SQLiteDbFactory>()
                .As<ISQLiteDbFactory>()
                .SingleInstance();

            builder.RegisterType<SQLiteDbManager>()
                .As<ISQLiteDbManager>()
                .SingleInstance();

            builder.RegisterType<DbFilePath>()
                .AsSelf()
                .InstancePerDependency();
        }
    }
}