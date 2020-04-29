using Autofac;
using Newbe.Claptrap.Preview.Abstractions;
using Newbe.Claptrap.Preview.Impl.Modules;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite.Module
{
    public class SQLiteStorageModule : StorageSupportModule
    {
        public SQLiteStorageModule()
        {
            EventStoreType = typeof(SQLiteEventStore);
            StateStoreType = typeof(SQLiteStateStore);
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SQLiteDbFactory>()
                .As<ISQLiteDbFactory>()
                .SingleInstance();

            builder.RegisterType<SQLiteDbManager>()
                .As<ISQLiteDbManager>()
                .SingleInstance();
        }
    }
}