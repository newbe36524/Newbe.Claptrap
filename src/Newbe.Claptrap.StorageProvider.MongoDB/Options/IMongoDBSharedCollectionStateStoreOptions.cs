using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IMongoDBSharedCollectionStateStoreOptions :
        IMongoDBStateLoaderOptions,
        IMongoDBStateSaverOptions,
        IMongoDBMigrationOptions,
        IBatchEventSaverOptions
    {
        string DatabaseName { get; }
        string CollectionName { get; }
        string ConnectionName { get; }
    }
}