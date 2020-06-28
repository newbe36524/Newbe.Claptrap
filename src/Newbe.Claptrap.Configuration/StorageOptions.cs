namespace Newbe.Claptrap
{
    public class StorageOptions
    {
        public RelationLocatorStrategy Strategy { get; set; }
        public DatabaseType DatabaseType { get; set; }
        public string ConnectionName { get; set; } = Defaults.ConnectionName;
        public string SchemaName { get; set; } = Defaults.SchemaName;
        public string TableName { get; set; } = null!;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = null!;
        public int? InsertManyWindowCount { get; set; } = null!;
    }
}