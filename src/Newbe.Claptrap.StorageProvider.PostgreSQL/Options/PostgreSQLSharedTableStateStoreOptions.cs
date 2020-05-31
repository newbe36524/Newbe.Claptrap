namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public class PostgreSQLSharedTableStateStoreOptions : IPostgreSQLSharedTableStateStoreOptions
    {
        public PostgreSQLStateStoreStrategy PostgreSQLStateStoreStrategy { get; } =
            PostgreSQLStateStoreStrategy.SharedTable;

        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public string SchemaName { get; set; } = "claptrap";
        public string StateTableName { get; set; } = "states";
        public string ConnectionName { get; set; } = "claptrap";
        public bool IsAutoMigrationEnabled { get; } = true;
    }
}