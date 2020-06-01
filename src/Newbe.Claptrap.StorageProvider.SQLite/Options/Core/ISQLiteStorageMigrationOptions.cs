using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options.Core
{
    public interface ISQLiteStorageMigrationOptions :
        IAutoMigrationOptions,
        ISQLiteStorageProviderOptions
    {
    }
}