using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public class MySqlStateStoreOptions : IMySqlStateStoreOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public bool IsAutoMigrationEnabled { get; } = true;
        public IRelationalStateStoreLocator RelationalStateStoreLocator { get; set; } = null!;
    }
}