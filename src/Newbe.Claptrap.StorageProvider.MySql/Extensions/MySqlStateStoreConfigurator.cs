using System;
using Newbe.Claptrap.StorageProvider.MySql.Options;

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

        public MySqlStateStoreConfigurator SharedTable(Action<MySqlSharedTableStateStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MySqlSharedTableStateStoreOptions();
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
    }
}