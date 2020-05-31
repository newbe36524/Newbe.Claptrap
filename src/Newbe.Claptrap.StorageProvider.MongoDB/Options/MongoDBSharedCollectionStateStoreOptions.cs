namespace Newbe.Claptrap.StorageProvider.MongoDB.Options
{
    public class MongoDBSharedCollectionStateStoreOptions : IMongoDBSharedCollectionStateStoreOptions
    {
        public MongoDBStateStoreStrategy MongoDBStateStoreStrategy { get; } =
            MongoDBStateStoreStrategy.SharedCollection;

        public int? InsertManyWindowTimeInMilliseconds { get; set; } = 50;
        public int? InsertManyWindowCount { get; set; } = 1000;
        public string DatabaseName { get; set; } = "claptrap";
        public string CollectionName { get; set; } = "states";
        public string ConnectionName { get; set; } = "claptrap";
        public bool IsAutoMigrationEnabled { get; } = true;
    }
}