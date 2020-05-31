namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IStateStoreStrategyOptions
        : IStorageProviderOptions
    {
        MongoDBStateStoreStrategy MongoDBStateStoreStrategy { get; }
    }
}