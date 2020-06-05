using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class SQLiteRelationalStateStoreOptions
        : ISQLiteRelationalStateStoreOptions
    {
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalStateStoreLocator RelationalStateStoreLocator { get; set; } = null!;
    }
}