using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteStateStoreOptions : ISQLiteStateStoreOptions
    {
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalStateStoreLocator RelationalStateStoreLocator { get; set; } = null!;
        public int? InsertManyWindowTimeInMilliseconds { get; } = 50;
        public int? InsertManyWindowCount { get; } = 100;
        public int? WorkerCount { get; } = 1;
    }
}