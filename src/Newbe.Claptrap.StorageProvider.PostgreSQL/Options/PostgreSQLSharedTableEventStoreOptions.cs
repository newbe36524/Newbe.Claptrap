namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public class PostgreSQLSharedTableEventStoreOptions : IPostgreSQLSharedTableEventStoreOptions
    {
        public PostgreSQLEventStoreStrategy PostgreSQLEventStoreStrategy { get; } = PostgreSQLEventStoreStrategy.SharedTable;
        public string SchemaName { get; set; } = "claptrap";
        public string EventTableName { get; set; } = "events";
        public string ConnectionName { get; set; } = "claptrap";
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public bool IsAutoMigrationEnabled { get; set; } = true;
    }
}