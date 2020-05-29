namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public class MySqlSharedTableEventStoreOptions : IMySqlSharedTableEventStoreOptions
    {
        public EventStoreStrategy EventStoreStrategy { get; set; }
        public string SchemaName { get; set; }
        public string EventTableName { get; set; }
        public string SharedTableEventStoreDbName { get; set; }
        public int? InsertManyWindowTimeInMilliseconds { get; set; }
        public int? InsertManyWindowCount { get; set; }
        public bool IsAutoMigrationEnabled { get; set; }
    }
}