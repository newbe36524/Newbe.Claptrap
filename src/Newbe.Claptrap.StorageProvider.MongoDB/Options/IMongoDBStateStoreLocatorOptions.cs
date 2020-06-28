using Newbe.Claptrap.StorageProvider.MongoDB.StateStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IMongoDBStateStoreLocatorOptions
        : IBatchSaverOptions
    {
        IMongoDBStateStoreLocator MongoDBStateStoreLocator { get; }
    }
}