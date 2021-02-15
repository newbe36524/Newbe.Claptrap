using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IMongoDBEventStoreOptions :
        IMongoDBEventLoaderOptions,
        IMongoDBEventSaverOptions,
        IMongoDBMigrationOptions,
        IMongoDBEventStoreLocatorOptions,
        IBatchSaverOptions
    {
    }
}