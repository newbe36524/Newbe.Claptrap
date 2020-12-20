using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteEventStoreOptions : ISQLiteEventStoreOptions
    {
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalEventStoreLocator RelationalEventStoreLocator { get; set; } = null!;
        public int? InsertManyWindowTimeInMilliseconds { get; } = 50;
        public const int SQLiteMaxVariablesCount = 999;
        public int? InsertManyWindowCount { get; } = 10000;
        public int? WorkerCount { get; } = 5;
        public bool? EnableAutoScale { get; } = true;
    }
}