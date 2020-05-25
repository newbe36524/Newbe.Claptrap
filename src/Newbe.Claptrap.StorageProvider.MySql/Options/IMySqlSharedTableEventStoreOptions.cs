using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IMySqlSharedTableEventStoreOptions :
        IRelationalEventLoaderOptions,
        IRelationalEventSaverOptions,
        IBatchEventSaverOptions,
        IAutoMigrationOptions
    {
        string SchemaName { get; }
        string EventTableName { get; }
        string SharedTableEventStoreDbName { get; }
    }

    public class MySqlSharedTableEventStoreOptions : IMySqlSharedTableEventStoreOptions
    {
        public Relational.EventStore.EventStoreStrategy EventStoreStrategy { get; set; }
        public string SchemaName { get; set; }
        public string EventTableName { get; set; }
        public string SharedTableEventStoreDbName { get; set; }
        public int? InsertManyWindowTimeInMilliseconds { get; set; }
        public int? InsertManyWindowCount { get; set; }
        public bool IsAutoMigrationEnabled { get; set; }
    }
}