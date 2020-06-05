using System;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Extensions
{
    public class PostgreSQLEvenStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public PostgreSQLEvenStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private PostgreSQLEvenStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public PostgreSQLEvenStoreConfigurator SharedTable(Action<PostgreSQLEventStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new PostgreSQLEventStoreOptions
                {
                    RelationalEventStoreLocator = new RelationalEventStoreLocator(
                        schemaName: Defaults.SchemaName,
                        connectionName: Defaults.ConnectionName,
                        eventTableName: Defaults.EventTableName)
                };
                options(eventOptions);
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
            return this;
        }

        public PostgreSQLEvenStoreConfigurator SharedTable()
        {
            return SharedTable(options => { });
        }

        public PostgreSQLEvenStoreConfigurator CustomLocator(
            string? schemaName = null,
            string? connectionName = null,
            string? eventTableName = null,
            Func<IClaptrapIdentity, string>? schemaNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? eventTableNameFunc = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new PostgreSQLEventStoreOptions
                {
                    RelationalEventStoreLocator = new RelationalEventStoreLocator(
                        schemaName,
                        connectionName,
                        eventTableName,
                        schemaNameFunc,
                        connectionNameFunc,
                        eventTableNameFunc),
                };
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
            return this;
        }
    }
}