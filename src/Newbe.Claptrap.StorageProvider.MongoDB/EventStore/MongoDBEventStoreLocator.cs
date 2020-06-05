using System;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore
{
    public class MongoDBEventStoreLocator : IMongoDBEventStoreLocator
    {
        private readonly Func<IClaptrapIdentity, string>? _connectionNameFunc;
        private readonly Func<IClaptrapIdentity, string>? _eventCollectionNameFunc;
        private readonly Func<IClaptrapIdentity, string>? _databaseNameFunc;
        private readonly string? _databaseName;
        private readonly string? _connectionName;
        private readonly string? _eventCollectionName;

        public MongoDBEventStoreLocator(
            string? databaseName = null,
            string? connectionName = null,
            string? eventCollectionName = null,
            Func<IClaptrapIdentity, string>? databaseNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? eventCollectionNameFunc = null)
        {
            _databaseName = databaseName;
            _connectionName = connectionName;
            _eventCollectionName = eventCollectionName;
            _databaseNameFunc = databaseNameFunc;
            _connectionNameFunc = connectionNameFunc;
            _eventCollectionNameFunc = eventCollectionNameFunc;
        }

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return _connectionNameFunc?.Invoke(identity) ?? _connectionName!;
        }

        public string GetDatabaseName(IClaptrapIdentity identity)
        {
            return _databaseNameFunc?.Invoke(identity) ?? _databaseName!;
        }

        public string GetEventCollectionName(IClaptrapIdentity identity)
        {
            return _eventCollectionNameFunc?.Invoke(identity) ?? _eventCollectionName!;
        }
    }
}