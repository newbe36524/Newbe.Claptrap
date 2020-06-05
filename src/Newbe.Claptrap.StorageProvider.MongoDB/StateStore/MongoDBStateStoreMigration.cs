using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.EventStore;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore
{
    public class MongoDBStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public MongoDBStateStoreMigration(
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IMongoDBStateStoreLocatorOptions options)
        {
            var locator = options.MongoDBStateStoreLocator;
            var connectionName = locator.GetConnectionName(identity);
            var databaseName = locator.GetDatabaseName(identity);
            var stateCollectionName = locator.GetStateCollectionName(identity);
            var migration = new InternalMongoDBSharedCollectionEventStoreMigration(
                connectionName,
                databaseName,
                stateCollectionName,
                dbFactory);
            var migrationKey =
                $"{nameof(MongoDBEventStoreMigration)}_{stateCollectionName}_{databaseName}_{connectionName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }


        public Task MigrateAsync()
        {
            return _migrationTask;
        }

        private class InternalMongoDBSharedCollectionEventStoreMigration : IStorageMigration
        {
            private readonly string _connectionName;
            private readonly string _databaseName;
            private readonly string _collectionName;
            private readonly IDbFactory _dbFactory;

            public InternalMongoDBSharedCollectionEventStoreMigration(
                string connectionName,
                string databaseName,
                string collectionName,
                IDbFactory dbFactory)
            {
                _connectionName = connectionName;
                _databaseName = databaseName;
                _collectionName = collectionName;
                _dbFactory = dbFactory;
            }

            public Task MigrateAsync()
            {
                var client = _dbFactory.GetConnection(_connectionName);
                var db = client.GetDatabase(_databaseName);
                var collection = db.GetCollection<MongoStateEntity>(_collectionName);
                var builder = Builders<MongoStateEntity>.IndexKeys;

                var indexKeysDefinition = builder.Combine(builder.Descending(x => x.version),
                    builder.Descending(x => x.claptrap_id),
                    builder.Descending(x => x.claptrap_type_code));
                return collection.Indexes.CreateOneAsync(new CreateIndexModel<MongoStateEntity>(
                    indexKeysDefinition, new CreateIndexOptions
                    {
                        Unique = true,
                    }));
            }
        }
    }
}