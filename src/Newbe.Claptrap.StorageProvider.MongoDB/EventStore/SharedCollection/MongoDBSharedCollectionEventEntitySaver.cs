using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Tools;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection
{
    public class MongoDBSharedCollectionEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly IBatchOperator<EventEntity> _batchOperator;

        public MongoDBSharedCollectionEventEntitySaver(
            IMongoDBSharedCollectionEventStoreOptions options,
            BatchOperator<EventEntity>.Factory batchOperatorFactory,
            IDbFactory dbFactory,
            IBatchOperatorContainer batchOperatorContainer)
        {
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                new SharedTableEventBatchOperatorKey(options), () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>
                    {
                        BufferCount = options.InsertManyWindowCount,
                        BufferTime = options.InsertManyWindowTimeInMilliseconds.HasValue
                            ? TimeSpan.FromMilliseconds(options.InsertManyWindowTimeInMilliseconds.Value)
                            : default,
                        DoManyFunc = entities => SaveManyCoreMany(dbFactory, options, entities)
                    }));
        }

        public Task SaveAsync(EventEntity entity)
        {
            return _batchOperator.CreateTask(entity);
        }

        private readonly struct SharedTableEventBatchOperatorKey : IBatchOperatorKey
        {
            private readonly IMongoDBSharedCollectionEventStoreOptions _options;

            public SharedTableEventBatchOperatorKey(
                IMongoDBSharedCollectionEventStoreOptions options)
            {
                _options = options;
            }

            public string AsStringKey()
            {
                return
                    $"{nameof(MongoDBSharedCollectionEventEntitySaver)}-{_options.ConnectionName}-{_options.DatabaseName}-{_options.CollectionName}";
            }
        }

        private async Task SaveManyCoreMany(
            IDbFactory dbFactory,
            IMongoDBSharedCollectionEventStoreOptions options,
            IEnumerable<EventEntity> entities)
        {
            var array = entities as EventEntity[] ?? entities.ToArray();
            var items = array
                .Select(x => new SharedCollectionEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                })
                .ToArray();

            var client = dbFactory.GetConnection(options.ConnectionName);
            var db = client.GetDatabase(options.DatabaseName);
            var collection = db.GetCollection<SharedCollectionEventEntity>(options.CollectionName);
            var insertOneModels = items.Select(x => new InsertOneModel<SharedCollectionEventEntity>(x));
            await collection.BulkWriteAsync(insertOneModels);
        }
    }
}