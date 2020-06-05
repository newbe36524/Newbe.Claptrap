using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteRelationalEventStoreOptions :
        ISQLiteRelationalEventStoreOptions
    {
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalEventStoreLocator RelationalEventStoreLocator { get; set; } = null!;
        public int? InsertManyWindowTimeInMilliseconds { get; } = 20;
        public int? InsertManyWindowCount { get; } = 1000;
    }
}