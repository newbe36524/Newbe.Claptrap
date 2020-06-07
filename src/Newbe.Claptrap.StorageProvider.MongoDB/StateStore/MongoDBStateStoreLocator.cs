using System;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore
{
    public class MongoDBStateStoreLocator : IMongoDBStateStoreLocator
    {
        public Func<IClaptrapIdentity, string>? ConnectionNameFunc { get; set; } = null!;
        public Func<IClaptrapIdentity, string>? StateCollectionNameFunc { get; set; } = null!;
        public Func<IClaptrapIdentity, string>? DatabaseNameFunc { get; set; } = null!;
        public string? DatabaseName { get; set; } = null!;
        public string? ConnectionName { get; set; } = null!;
        public string? StateCollectionName { get; set; } = null!;

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return ConnectionNameFunc?.Invoke(identity) ?? ConnectionName!;
        }

        public string GetDatabaseName(IClaptrapIdentity identity)
        {
            return DatabaseNameFunc?.Invoke(identity) ?? DatabaseName!;
        }

        public string GetStateCollectionName(IClaptrapIdentity identity)
        {
            return StateCollectionNameFunc?.Invoke(identity) ?? StateCollectionName!;
        }
    }
}