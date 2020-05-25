using System;
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
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.StateLoaderFactoryType = typeof(MySqlStateStoreFactory);
                design.StateSaverFactoryType = typeof(MySqlStateStoreFactory);
            });

        public static IClaptrapBootstrapperBuilder UseMySqlAsEventStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.EventLoaderFactoryType = typeof(RelationalEventStoreFactory);
                design.EventSaverFactoryType = typeof(RelationalEventStoreFactory);
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
                design.StorageProviderOptions = new StorageProviderOptions
                {
                    EventLoaderOptions = mysqlOptions,
                    EventSaverOptions = mysqlOptions,
                };
            });
    }
}