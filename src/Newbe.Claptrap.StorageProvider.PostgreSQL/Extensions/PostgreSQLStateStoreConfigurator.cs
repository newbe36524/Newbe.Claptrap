using System;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;

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

        public PostgreSQLStateStoreConfigurator SharedTable(Action<PostgreSQLSharedTableStateStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new PostgreSQLSharedTableStateStoreOptions();
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
    }
}