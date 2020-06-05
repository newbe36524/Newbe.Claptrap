using System;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore
{
    public class MongoDBStateStoreLocator : IMongoDBStateStoreLocator
    {
        private readonly Func<IClaptrapIdentity, string>? _connectionNameFunc;
        private readonly Func<IClaptrapIdentity, string>? _stateCollectionNameFunc;
        private readonly Func<IClaptrapIdentity, string>? _databaseNameFunc;
        private readonly string? _databaseName;
        private readonly string? _connectionName;
        private readonly string? _stateCollectionName;

        public MongoDBStateStoreLocator(
            string? databaseName = null,
            string? connectionName = null,
            string? stateCollectionName = null,
            Func<IClaptrapIdentity, string>? databaseNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? stateCollectionNameFunc = null)
        {
            _databaseName = databaseName;
            _connectionName = connectionName;
            _stateCollectionName = stateCollectionName;
            _databaseNameFunc = databaseNameFunc;
            _connectionNameFunc = connectionNameFunc;
            _stateCollectionNameFunc = stateCollectionNameFunc;
        }

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return _connectionNameFunc?.Invoke(identity) ?? _connectionName!;
        }

        public string GetDatabaseName(IClaptrapIdentity identity)
        {
            return _databaseNameFunc?.Invoke(identity) ?? _databaseName!;
        }

        public string GetStateCollectionName(IClaptrapIdentity identity)
        {
            return _stateCollectionNameFunc?.Invoke(identity) ?? _stateCollectionName!;
        }
    }
}