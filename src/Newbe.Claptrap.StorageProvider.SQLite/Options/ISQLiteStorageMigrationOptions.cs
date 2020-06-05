using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteStorageMigrationOptions :
        IAutoMigrationOptions,
        ISQLiteStorageProviderOptions
    {
    }
}