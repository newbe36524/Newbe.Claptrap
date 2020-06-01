namespace Newbe.Claptrap.StorageProvider.SQLite.Options.Core
{
    public interface ISQLiteStateStoreStrategyOptions
        : IStorageProviderOptions
    {
        SQLiteStateStoreStrategy SQLiteStateStoreStrategy { get; }
    }
}