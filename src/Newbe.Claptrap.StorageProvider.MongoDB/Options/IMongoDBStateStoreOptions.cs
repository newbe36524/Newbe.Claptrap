using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IMongoDBStateStoreOptions :
        IMongoDBStateLoaderOptions,
        IMongoDBStateSaverOptions,
        IMongoDBMigrationOptions,
        IMongoDBStateStoreLocatorOptions,
        IBatchSaverOptions
    {
    
    }
}