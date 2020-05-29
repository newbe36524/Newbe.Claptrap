using System;
using Newbe.Claptrap.StorageProvider.MySql.Options;

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

        public MySqlEvenStoreConfigurator SharedTable(Action<MySqlSharedTableEventStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new MySqlSharedTableEventStoreOptions();
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
    }
}