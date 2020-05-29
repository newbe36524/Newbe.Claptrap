namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteOneIdOneFileEventStoreOptions : ISQLiteOneIdOneFileEventStoreOptions
    {
        public SQLiteEventStoreStrategy SQLiteEventStoreStrategy { get; } = SQLiteEventStoreStrategy.OneIdOneFile;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public string EventTableName { get; set; } = "events";
    }
}