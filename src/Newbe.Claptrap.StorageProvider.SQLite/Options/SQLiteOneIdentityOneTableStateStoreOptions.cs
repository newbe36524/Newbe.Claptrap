using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteOneIdentityOneTableStateStoreOptions : ISQLiteOneIdentityOneTableStateStoreOptions
    {
        public StateStoreStrategy StateStoreStrategy { get; } = StateStoreStrategy.OneIdentityOneTable;
        public bool IsAutoMigrationEnabled { get; } = true;
        public string StateTableName { get; } = "states";
    }
}