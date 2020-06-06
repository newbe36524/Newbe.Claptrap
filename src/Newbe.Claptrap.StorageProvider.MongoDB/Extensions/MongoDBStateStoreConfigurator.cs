using System;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.MongoDB.StateStore;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

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

        public MongoDBStateStoreConfigurator SharedCollection(
            Action<MongoDBStateStoreOptions>? action = null)
            =>
                UseLocator(new MongoDBStateStoreLocator
                {
                    DatabaseName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateCollectionName = Defaults.StateTableName
                }, action);


        public MongoDBStateStoreConfigurator OneIdOneCollection(
            Action<MongoDBStateStoreOptions>? action = null)
            =>
                UseLocator(new MongoDBStateStoreLocator
                {
                    DatabaseName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateCollectionNameFunc = id => $"{id.TypeCode}_{id.Id}_{Defaults.StateTableName}"
                }, action);


        public MongoDBStateStoreConfigurator OneTypeOneCollection(
            Action<MongoDBStateStoreOptions>? action = null)
            =>
                UseLocator(new MongoDBStateStoreLocator
                {
                    DatabaseName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateCollectionNameFunc = id => $"{id.TypeCode}_{Defaults.StateTableName}"
                }, action);

        private MongoDBStateStoreConfigurator UseLocator(
            IMongoDBStateStoreLocator locator,
            Action<MongoDBStateStoreOptions>? action = null
        )
            => ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MongoDBStateStoreOptions
                {
                    MongoDBStateStoreLocator = locator,
                };
                action?.Invoke(stateOptions);
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
    }
}