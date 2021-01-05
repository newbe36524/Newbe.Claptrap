using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public class MySqlEventStoreOptions : IMySqlEventStoreOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1_000;
        public int? WorkerCount { get; set; } = 5;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalEventStoreLocator RelationalEventStoreLocator { get; set; } = null!;
    }
}