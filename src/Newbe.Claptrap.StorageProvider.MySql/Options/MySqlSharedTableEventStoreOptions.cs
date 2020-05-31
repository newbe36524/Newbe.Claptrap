namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public class MySqlSharedTableEventStoreOptions : IMySqlSharedTableEventStoreOptions
    {
        public MySqlEventStoreStrategy MySqlEventStoreStrategy { get; } = MySqlEventStoreStrategy.SharedTable;
        public string SchemaName { get; set; } = "claptrap";
        public string EventTableName { get; set; } = "events";
        public string ConnectionName { get; set; } = "claptrap";
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public bool IsAutoMigrationEnabled { get; set; } = true;
    }
}