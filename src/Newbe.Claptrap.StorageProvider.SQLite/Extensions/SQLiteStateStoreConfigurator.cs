using System;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Extensions
{
    public class SQLiteStateStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public SQLiteStateStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private SQLiteStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public SQLiteStateStoreConfigurator OneIdOneTable()
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new SQLiteOneIdOneFileStateStoreOptions();
                providerOptions.StateLoaderOptions = eventOptions;
                providerOptions.StateSaverOptions = eventOptions;
            });
            return this;
        }
    }
}