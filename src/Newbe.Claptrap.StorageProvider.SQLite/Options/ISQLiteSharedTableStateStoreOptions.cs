using Newbe.Claptrap.StorageProvider.SQLite.Options.Core;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteSharedTableStateStoreOptions :
        ISQLiteStateLoaderOptions,
        ISQLiteStateSaverOptions,
        ISQLiteStorageMigrationOptions
    {
        string ConnectionName { get; }
        string StateTableName { get; }
    }
}