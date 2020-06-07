using System;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore
{
    public class MongoDBEventStoreLocator : IMongoDBEventStoreLocator
    {
        public Func<IClaptrapIdentity, string>? ConnectionNameFunc { get; set; } = null!;
        public Func<IClaptrapIdentity, string>? EventCollectionNameFunc { get; set; } = null!;
        public Func<IClaptrapIdentity, string>? DatabaseNameFunc { get; set; } = null!;
        public string? DatabaseName { get; set; } = null!;
        public string? ConnectionName { get; set; } = null!;
        public string? EventCollectionName { get; set; } = null!;

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return ConnectionNameFunc?.Invoke(identity) ?? ConnectionName!;
        }

        public string GetDatabaseName(IClaptrapIdentity identity)
        {
            return DatabaseNameFunc?.Invoke(identity) ?? DatabaseName!;
        }

        public string GetEventCollectionName(IClaptrapIdentity identity)
        {
            return EventCollectionNameFunc?.Invoke(identity) ?? EventCollectionName!;
        }
    }
}