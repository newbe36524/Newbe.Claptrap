using System;
using Newbe.Claptrap.StorageProvider.MongoDB.EventStore;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.MongoDB.StateStore;
using Newbe.Claptrap.StorageProvider.Relational;

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

        public MongoDBEvenStoreConfigurator SharedCollection(Action<MongoDBEventStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new MongoDBEventStoreOptions
                {
                    MongoDBEventStoreLocator = new MongoDBEventStoreLocator(
                        databaseName: Defaults.SchemaName,
                        connectionName: Defaults.ConnectionName,
                        eventCollectionName: Defaults.EventTableName)
                };
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

        public MongoDBEvenStoreConfigurator CustomLocator(
            string? databaseName = null,
            string? connectionName = null,
            string? eventCollectionName = null,
            Func<IClaptrapIdentity, string>? databaseNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? eventCollectionNameFunc = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MongoDBEventStoreOptions
                {
                    MongoDBEventStoreLocator = new MongoDBEventStoreLocator(
                        databaseName,
                        connectionName,
                        eventCollectionName,
                        databaseNameFunc,
                        connectionNameFunc,
                        eventCollectionNameFunc),
                };
                providerOptions.EventLoaderOptions = stateOptions;
                providerOptions.EventSaverOptions = stateOptions;
            });
            return this;
        }
    }
}