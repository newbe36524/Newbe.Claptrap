using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public class PostgreSQLStateStoreOptions : IPostgreSQLStateStoreOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public int? WorkerCount { get; } = 2;
        public bool? EnableAutoScale { get; set; } = false;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalStateStoreLocator RelationalStateStoreLocator { get; set; } = null!;
    }
}