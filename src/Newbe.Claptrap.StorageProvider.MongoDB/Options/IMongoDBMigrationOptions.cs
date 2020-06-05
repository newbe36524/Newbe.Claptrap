using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public interface IMongoDBMigrationOptions :
        IAutoMigrationOptions,
        IMongoDBStorageProviderOptions
    {
    }
}