using System;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteSharedTableEventStoreOptions :
        ISQLiteSharedTableEventStoreOptions
    {
        public SQLiteEventStoreStrategy SQLiteEventStoreStrategy { get; } = SQLiteEventStoreStrategy.SharedTable;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public string ConnectionName { get; } = "shared/claptrap.events.db";
        public string EventTableName { get; } = "events";
        public int? InsertManyWindowTimeInMilliseconds { get; } = 20;
        public int? InsertManyWindowCount { get; } = 1000;
    }
}