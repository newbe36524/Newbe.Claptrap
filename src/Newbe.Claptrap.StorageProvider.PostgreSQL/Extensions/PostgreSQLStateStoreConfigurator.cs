using System;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Extensions
{
    public class PostgreSQLStateStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public PostgreSQLStateStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private PostgreSQLStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public PostgreSQLStateStoreConfigurator SharedTable(Action<PostgreSQLStateStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new PostgreSQLStateStoreOptions();
                options(stateOptions);
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
            return this;
        }

        public PostgreSQLStateStoreConfigurator SharedTable()
        {
            return SharedTable(options => { });
        }
        
        public PostgreSQLStateStoreConfigurator CustomLocator(
            string? schemaName = null,
            string? connectionName = null,
            string? stateTableName = null,
            Func<IClaptrapIdentity, string>? schemaNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? stateTableNameFunc = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new PostgreSQLStateStoreOptions
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