using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public class IsqLiteStateStoreOptions
        : ISQLiteStateStoreOptions
    {
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalStateStoreLocator RelationalStateStoreLocator { get; set; } = null!;
    }
}