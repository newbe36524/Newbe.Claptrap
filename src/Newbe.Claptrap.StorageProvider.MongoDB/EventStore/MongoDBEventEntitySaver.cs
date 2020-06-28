using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Tools;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore
{
    public class MongoDBEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly IBatchOperator<EventEntity> _batchOperator;
        private readonly string _connectionName;
        private readonly string _databaseName;
        private readonly string _eventCollectionName;

        public MongoDBEventEntitySaver(
            IMongoDBEventStoreOptions options,
            BatchOperator<EventEntity>.Factory batchOperatorFactory,
            IDbFactory dbFactory,
            IClaptrapIdentity identity,
            IBatchOperatorContainer batchOperatorContainer)
        {
            var locator = options.MongoDBEventStoreLocator;
            _connectionName = locator.GetConnectionName(identity);
            _databaseName = locator.GetDatabaseName(identity);
            _eventCollectionName = locator.GetEventCollectionName(identity);

            var operatorKey = new BatchOperatorKey()
                .With(nameof(MongoDBEventEntitySaver))
                .With(_connectionName)
                .With(_databaseName)
                .With(_eventCollectionName);
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>(options)
                    {
                        DoManyFunc = (entities, cacheData) => SaveManyCoreMany(dbFactory, entities)
                    }));
        }

        public Task SaveAsync(EventEntity entity)
        {
            return _batchOperator.CreateTask(entity);
        }

        private async Task SaveManyCoreMany(
            IDbFactory dbFactory,
            IEnumerable<EventEntity> entities)
        {
            var array = entities as EventEntity[] ?? entities.ToArray();
            var items = array
                .Select(x => new MongoEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                })
                .ToArray();

            var client = dbFactory.GetConnection(_connectionName);
            var db = client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoEventEntity>(_eventCollectionName);
            var insertOneModels = items.Select(x => new InsertOneModel<MongoEventEntity>(x));
            await collection.BulkWriteAsync(insertOneModels);
        }
    }
}