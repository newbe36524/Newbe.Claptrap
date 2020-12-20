using Newbe.Claptrap.StorageProvider.MongoDB.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public class MongoDBEventStoreOptions : IMongoDBEventStoreOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1500;
        public int? WorkerCount { get; } = 5;
        public bool? EnableAutoScale { get; set; } = true;
        public bool IsAutoMigrationEnabled { get; set; } = true;
        public IMongoDBEventStoreLocator MongoDBEventStoreLocator { get; set; } = null!;
    }
}