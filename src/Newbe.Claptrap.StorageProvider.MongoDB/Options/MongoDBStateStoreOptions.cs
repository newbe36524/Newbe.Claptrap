using Newbe.Claptrap.StorageProvider.MongoDB.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public class MongoDBStateStoreOptions : IMongoDBStateStoreOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public int? InsertManyMaxWindowCount { get; set; } = 2000;
        public int? InsertManyMinWindowCount { get; set; } = 100;
        public bool? EnableAutoScale { get; set; } = false;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IMongoDBStateStoreLocator MongoDBStateStoreLocator { get; set; } = null!;
    }
}