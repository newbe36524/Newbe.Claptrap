namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteOneIdOneFileStateStoreOptions :
        ISQLiteStateLoaderOptions,
        ISQLiteStateSaverOptions,
        ISQLiteStorageMigrationOptions
    {
        string StateTableName { get; }
    }
}