

using Newbe.Claptrap.StorageProvider.MongoDB.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IMongoDBStateStoreLocatorOptions
        : IStorageProviderOptions
    {
        IMongoDBStateStoreLocator MongoDBStateStoreLocator { get; }
    }
}