using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IMongoDBSharedCollectionEventStoreOptions :
        IMongoDBEventLoaderOptions,
        IMongoDBEventSaverOptions,
        IMongoDBMigrationOptions,
        IBatchEventSaverOptions
    {
        string DatabaseName { get; }
        string CollectionName { get; }
        string ConnectionName { get; }
    }
}