namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteStateStoreStrategyOptions
        : IStorageProviderOptions
    {
        SQLiteStateStoreStrategy SQLiteStateStoreStrategy { get; }
    }
}