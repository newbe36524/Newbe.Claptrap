using System;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Extensions
{
    public class MongoDBStateStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public MongoDBStateStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private MongoDBStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public MongoDBStateStoreConfigurator SharedCollection(Action<MongoDBSharedCollectionStateStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MongoDBSharedCollectionStateStoreOptions();
                options(stateOptions);
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
            return this;
        }

        public MongoDBStateStoreConfigurator SharedCollection()
        {
            return SharedCollection(options => { });
        }
    }
}