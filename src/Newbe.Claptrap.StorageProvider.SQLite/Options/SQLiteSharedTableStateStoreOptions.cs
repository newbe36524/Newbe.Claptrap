namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteSharedTableStateStoreOptions
        : ISQLiteSharedTableStateStoreOptions
    {
        public SQLiteStateStoreStrategy SQLiteStateStoreStrategy { get; } = SQLiteStateStoreStrategy.SharedTable;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public string ConnectionName { get; set; } = "shared/states.db";
        public string StateTableName { get; set; } = "states";
    }
}