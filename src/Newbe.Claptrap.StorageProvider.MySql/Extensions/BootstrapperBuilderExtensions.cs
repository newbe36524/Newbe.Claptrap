using Newbe.Claptrap.StorageProvider.MySql;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using EventStoreStrategy = Newbe.Claptrap.StorageProvider.Relational.EventStore.EventStoreStrategy;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseMySqlAsStateStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder
                .ConfigureClaptrapDesign(
                    x => x.StateLoaderFactoryType == null,
                    x =>
                        x.StateLoaderFactoryType = typeof(MySqlStateStoreFactory))
                .ConfigureClaptrapDesign(
                    x => x.StateSaverFactoryType == null,
                    x =>
                        x.StateSaverFactoryType = typeof(MySqlStateStoreFactory)
                );

        public static IClaptrapBootstrapperBuilder UseMySqlAsEventStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureClaptrapDesign(x =>
            {
                x.EventLoaderFactoryType = typeof(SQLiteStoreFactory);
                x.EventSaverFactoryType = typeof(SQLiteStoreFactory);
                var mysqlOptions = new MySqlSharedTableEventStoreOptions
                {
                    SchemaName = "claptrap",
                    EventStoreStrategy = EventStoreStrategy.SharedTable,
                    EventTableName = "claptrap_event_shared",
                    SharedTableEventStoreDbName = "mysql",
                    InsertManyWindowCount = 1000,
                    IsAutoMigrationEnabled = true,
                    InsertManyWindowTimeInMilliseconds = 20
                };
                x.ClaptrapStorageProviderOptions.EventLoaderOptions = mysqlOptions;
                x.ClaptrapStorageProviderOptions.EventSaverOptions = mysqlOptions;
            });
    }
}