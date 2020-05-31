using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore.SharedCollection
{
    public class MongoDBSharedCollectionStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public MongoDBSharedCollectionStateStoreMigration(
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IMongoDBSharedCollectionStateStoreOptions options)
        {
            var migration = new InternalMongoDBSharedCollectionEventStoreMigration(
                options.ConnectionName,
                options.DatabaseName,
                options.CollectionName,
                dbFactory);
            var migrationKey =
                $"{nameof(MongoDBSharedCollectionEventStoreMigration)}_{options.ConnectionName}_{options.DatabaseName}_{options.CollectionName}";
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
                var collection = db.GetCollection<SharedCollectionStateEntity>(_collectionName);
                var builder = Builders<SharedCollectionStateEntity>.IndexKeys;

                var indexKeysDefinition = builder.Combine(builder.Descending(x => x.version),
                    builder.Descending(x => x.claptrap_id),
                    builder.Descending(x => x.claptrap_type_code));
                return collection.Indexes.CreateOneAsync(new CreateIndexModel<SharedCollectionStateEntity>(
                    indexKeysDefinition, new CreateIndexOptions
                    {
                        Unique = true,
                    }));
            }
        }
    }
}