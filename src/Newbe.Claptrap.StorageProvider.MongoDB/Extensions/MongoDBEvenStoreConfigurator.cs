using System;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Extensions
{
    public class MongoDBEvenStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public MongoDBEvenStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private MongoDBEvenStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public MongoDBEvenStoreConfigurator SharedCollection(Action<MongoDBSharedCollectionEventStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new MongoDBSharedCollectionEventStoreOptions();
                options(eventOptions);
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
            return this;
        }

        public MongoDBEvenStoreConfigurator SharedCollection()
        {
            return SharedCollection(options => { });
        }
    }
}