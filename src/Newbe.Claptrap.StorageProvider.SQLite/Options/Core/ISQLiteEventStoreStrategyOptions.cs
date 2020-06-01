namespace Newbe.Claptrap.StorageProvider.SQLite.Options.Core
{
    public interface ISQLiteEventStoreStrategyOptions
        : IStorageProviderOptions
    {
        SQLiteEventStoreStrategy SQLiteEventStoreStrategy { get; }
    }
}