using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore
{
    public class MongoDBEventStoreMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly Task _migrationTask;

        public MongoDBEventStoreMigration(
            ILogger<MongoDBEventStoreMigration> logger,
            IDbFactory dbFactory,
            IMasterOrSelfIdentity masterOrSelfIdentity,
            IStorageMigrationContainer storageMigrationContainer,
            IMongoDBEventStoreOptions options)
        {
            var locator = options.MongoDBEventStoreLocator;
            var identity = masterOrSelfIdentity.Identity;
            var connectionName = locator.GetConnectionName(identity);
            var databaseName = locator.GetDatabaseName(identity);
            var eventCollectionName = locator.GetEventCollectionName(identity);
            var migration = new InternalMongoDBSharedCollectionEventStoreMigration(
                connectionName,
                databaseName,
                eventCollectionName,
                dbFactory);
            var migrationKey =
                $"{nameof(MongoDBEventStoreMigration)}_{connectionName}_{databaseName}_{eventCollectionName}";
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
                var collection = db.GetCollection<MongoEventEntity>(_collectionName);
                var builder = Builders<MongoEventEntity>.IndexKeys;

                var indexKeysDefinition = builder.Combine(builder.Descending(x => x.version),
                    builder.Descending(x => x.claptrap_id),
                    builder.Descending(x => x.claptrap_type_code));
                return collection.Indexes.CreateOneAsync(new CreateIndexModel<MongoEventEntity>(
                    indexKeysDefinition, new CreateIndexOptions
                    {
                        Unique = true,
                    }));
            }
        }
    }
}