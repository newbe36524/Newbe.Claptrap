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

        public MongoDBStateStoreConfigurator SharedCollection(Action<MongoDBStateStoreOptions> options)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MongoDBStateStoreOptions
                {
                    MongoDBStateStoreLocator = new MongoDBStateStoreLocator(
                        databaseName: Defaults.SchemaName,
                        connectionName: Defaults.ConnectionName,
                        stateCollectionName: Defaults.StateTableName)
                };
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

        public MongoDBStateStoreConfigurator CustomLocator(
            string? databaseName = null,
            string? connectionName = null,
            string? stateCollectionName = null,
            Func<IClaptrapIdentity, string>? databaseNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? stateCollectionNameFunc = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MongoDBStateStoreOptions
                {
                    MongoDBStateStoreLocator = new MongoDBStateStoreLocator(
                        databaseName,
                        connectionName,
                        stateCollectionName,
                        databaseNameFunc,
                        connectionNameFunc,
                        stateCollectionNameFunc),
                };
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateLoaderOptions = stateOptions;
            });
            return this;
        }
    }
}