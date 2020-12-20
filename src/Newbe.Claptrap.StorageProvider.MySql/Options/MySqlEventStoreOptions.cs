using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public class MySqlEventStoreOptions : IMySqlEventStoreOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 10_000;
        public int? WorkerCount { get; } = 5;
        public bool? EnableAutoScale { get; set; } = true;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalEventStoreLocator RelationalEventStoreLocator { get; set; } = null!;
    }
}