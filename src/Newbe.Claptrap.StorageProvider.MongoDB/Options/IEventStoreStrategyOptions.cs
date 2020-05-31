namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IEventStoreStrategyOptions
        : IStorageProviderOptions
    {
        MongoDBEventStoreStrategy MongoDBEventStoreStrategy { get; }
    }
}