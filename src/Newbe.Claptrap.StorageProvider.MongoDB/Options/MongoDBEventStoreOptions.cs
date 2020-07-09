using Newbe.Claptrap.StorageProvider.MongoDB.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public class MongoDBEventStoreOptions : IMongoDBEventStoreOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IMongoDBEventStoreLocator MongoDBEventStoreLocator { get; set; } = null!;
    }
}