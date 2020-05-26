using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteOneIdentityOneTableEventStoreOptions :
        IRelationalEventLoaderOptions,
        IRelationalEventSaverOptions,
        IAutoMigrationOptions
    {
        string EventTableName { get; }
    }

    public class SQLiteOneIdentityOneTableEventStoreOptions : ISQLiteOneIdentityOneTableEventStoreOptions
    {
        public EventStoreStrategy EventStoreStrategy { get; } = EventStoreStrategy.OneIdentityOneTable;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public string EventTableName { get; set; } = "events";
    }
}