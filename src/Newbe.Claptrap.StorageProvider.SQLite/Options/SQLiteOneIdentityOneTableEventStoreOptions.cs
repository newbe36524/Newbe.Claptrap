using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteOneIdentityOneTableEventStoreOptions : ISQLiteOneIdentityOneTableEventStoreOptions
    {
        public EventStoreStrategy EventStoreStrategy { get; } = EventStoreStrategy.OneIdentityOneTable;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public string EventTableName { get; set; } = "events";
    }
}