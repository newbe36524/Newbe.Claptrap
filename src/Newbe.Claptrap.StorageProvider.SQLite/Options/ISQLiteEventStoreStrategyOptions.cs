namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteEventStoreStrategyOptions
        : IStorageProviderOptions
    {
        SQLiteEventStoreStrategy SQLiteEventStoreStrategy { get; }
    }
}