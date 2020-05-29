
namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteOneIdOneFileStateStoreOptions : ISQLiteOneIdOneFileStateStoreOptions
    {
        public SQLiteStateStoreStrategy SQLiteStateStoreStrategy { get; } = SQLiteStateStoreStrategy.OneIdOneFile;
        public bool IsAutoMigrationEnabled { get; } = true;
        public string StateTableName { get; } = "states";
    }
}