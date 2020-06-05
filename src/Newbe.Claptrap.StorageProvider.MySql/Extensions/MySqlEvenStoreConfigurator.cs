using System;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.Extensions
{
    public class MySqlEvenStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public MySqlEvenStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private MySqlEvenStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public MySqlEvenStoreConfigurator SharedTable(Action<MySqlEventStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new MySqlEventStoreOptions
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

        public MySqlEvenStoreConfigurator SharedTable()
        {
            return SharedTable(options => { });
        }

        public MySqlEvenStoreConfigurator CustomLocator(
            string? schemaName = null,
            string? connectionName = null,
            string? eventTableName = null,
            Func<IClaptrapIdentity, string>? schemaNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? eventTableNameFunc = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new MySqlEventStoreOptions
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