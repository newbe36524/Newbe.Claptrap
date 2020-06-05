using System;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.Extensions
{
    public class MySqlStateStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public MySqlStateStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private MySqlStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public MySqlStateStoreConfigurator SharedTable(Action<MySqlStateStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MySqlStateStoreOptions
                {
                    RelationalStateStoreLocator = new RelationalStateStoreLocator(
                        schemaName: Defaults.SchemaName,
                        connectionName: Defaults.ConnectionName,
                        stateTableName: Defaults.StateTableName)
                };
                options(stateOptions);
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
            return this;
        }

        public MySqlStateStoreConfigurator SharedTable()
        {
            return SharedTable(options => { });
        }

        public MySqlStateStoreConfigurator CustomLocator(
            string? schemaName = null,
            string? connectionName = null,
            string? stateTableName = null,
            Func<IClaptrapIdentity, string>? schemaNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? stateTableNameFunc = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MySqlStateStoreOptions
                {
                    RelationalStateStoreLocator = new RelationalStateStoreLocator(
                        schemaName,
                        connectionName,
                        stateTableName,
                        schemaNameFunc,
                        connectionNameFunc,
                        stateTableNameFunc),
                };
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateLoaderOptions = stateOptions;
            });
            return this;
        }
    }
}