using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore
{
    public class MongoDBEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IBatchOperator<EventEntity> _batchOperator;
        private readonly string _connectionName;
        private readonly string _databaseName;
        private readonly string _eventCollectionName;

        public MongoDBEventEntitySaver(
            IMongoDBEventStoreOptions options,
            ChannelBatchOperator<EventEntity>.Factory batchOperatorFactory,
            IDbFactory dbFactory,
            IClaptrapIdentity identity,
            IBatchOperatorContainer batchOperatorContainer)
        {
            _dbFactory = dbFactory;
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
                        DoManyFunc = (entities, cacheData) => SaveManyAsync(entities),
                        DoManyFuncName = $"event batch saver for {operatorKey.AsStringKey()}"
                    }));
        }

        public Task SaveAsync(EventEntity entity)
        {
            var valueTask = _batchOperator.CreateTask(entity);
            if (valueTask.IsCompleted)
            {
                return Task.CompletedTask;
            }

            return valueTask.AsTask();
        }

        public async Task SaveManyAsync(IEnumerable<EventEntity> entities)
        {
            var items = entities
                .Select(x => new MongoEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                });

            var client = _dbFactory.GetConnection(_connectionName);
            var db = client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoEventEntity>(_eventCollectionName);
            await collection.InsertManyAsync(items, new InsertManyOptions
            {
                IsOrdered = false
            });
        }
    }
}