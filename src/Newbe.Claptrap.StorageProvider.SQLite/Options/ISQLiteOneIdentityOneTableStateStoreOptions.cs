using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteOneIdentityOneTableStateStoreOptions :
        IRelationalStateLoaderOptions,
        IRelationalStateSaverOptions,
        ISQLiteStorageMigrationOptions
    {
        string StateTableName { get; }
    }
}