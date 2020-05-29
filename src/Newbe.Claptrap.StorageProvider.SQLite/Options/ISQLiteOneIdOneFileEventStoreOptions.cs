namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteOneIdOneFileEventStoreOptions :
        ISQLiteEventLoaderOptions,
        ISQLiteEventSaverOptions,
        ISQLiteStorageMigrationOptions
    {
        string EventTableName { get; }
    }
}