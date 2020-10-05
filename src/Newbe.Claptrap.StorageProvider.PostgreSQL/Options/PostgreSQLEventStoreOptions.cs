using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public class PostgreSQLEventStoreOptions : IPostgreSQLEventStoreOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1_000;
        public int? InsertManyMaxWindowCount { get; set; } = 10_000;
        public int? InsertManyMinWindowCount { get; set; } = 1_000;
        public bool? EnableAutoScale { get; set; } = true;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IRelationalEventStoreLocator RelationalEventStoreLocator { get; set; } = null!;
    }
}