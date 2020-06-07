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
            Action<ClaptrapStorageProviderOptions>? optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public MongoDBEvenStoreConfigurator SharedCollection(
            Action<MongoDBEventStoreOptions>? action = null)
            =>
                UseLocator(new MongoDBEventStoreLocator
                {
                    DatabaseName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventCollectionName = Defaults.EventTableName
                }, action);


        public MongoDBEvenStoreConfigurator OneIdOneCollection(
            Action<MongoDBEventStoreOptions>? action = null)
            =>
                UseLocator(new MongoDBEventStoreLocator
                {
                    DatabaseName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventCollectionNameFunc = id => $"{id.TypeCode}_{id.Id}_{Defaults.EventTableName}"
                }, action);


        public MongoDBEvenStoreConfigurator OneTypeOneCollection(
            Action<MongoDBEventStoreOptions>? action = null)
            =>
                UseLocator(new MongoDBEventStoreLocator
                {
                    DatabaseName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventCollectionNameFunc = id => $"{id.TypeCode}_{Defaults.EventTableName}"
                }, action);

        private MongoDBEvenStoreConfigurator UseLocator(
            IMongoDBEventStoreLocator locator,
            Action<MongoDBEventStoreOptions>? action = null
        )
            => ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MongoDBEventStoreOptions
                {
                    MongoDBEventStoreLocator = locator,
                };
                action?.Invoke(stateOptions);
                providerOptions.EventLoaderOptions = stateOptions;
                providerOptions.EventSaverOptions = stateOptions;
            });
    }
}