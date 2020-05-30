using System;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;

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

        public PostgreSQLEvenStoreConfigurator SharedTable(Action<PostgreSQLSharedTableEventStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new PostgreSQLSharedTableEventStoreOptions();
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
    }
}