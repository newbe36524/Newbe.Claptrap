using Newbe.Claptrap.StorageProvider.MongoDB.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IMongoDBEventStoreLocatorOptions
        : IStorageProviderOptions
    {
        IMongoDBEventStoreLocator MongoDBEventStoreLocator { get; }
    }
}