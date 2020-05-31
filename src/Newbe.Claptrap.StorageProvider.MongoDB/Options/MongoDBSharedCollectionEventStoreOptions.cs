namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public class MongoDBSharedCollectionEventStoreOptions : IMongoDBSharedCollectionEventStoreOptions
    {
        public MongoDBEventStoreStrategy MongoDBEventStoreStrategy { get; } = MongoDBEventStoreStrategy.SharedCollection;
        public string DatabaseName { get; set; } = "claptrap";
        public string CollectionName { get; set; } = "events";
        public string ConnectionName { get; set; } = "claptrap";
        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public bool IsAutoMigrationEnabled { get; } = true;
    }
}