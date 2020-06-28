using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.Relational.Tools;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore
{
    public class MongoDBStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IBatchOperator<StateEntity> _batchOperator;
        private readonly string _stateCollectionName;
        private readonly string _databaseName;
        private readonly string _connectionName;

        public MongoDBStateEntitySaver(
            IClaptrapIdentity identity,
            BatchOperator<StateEntity>.Factory batchOperatorFactory,
            IDbFactory dbFactory,
            IMongoDBStateStoreLocatorOptions options,
            IBatchOperatorContainer batchOperatorContainer)
        {
            var locator = options.MongoDBStateStoreLocator;
            _connectionName = locator.GetConnectionName(identity);
            _databaseName = locator.GetDatabaseName(identity);
            _stateCollectionName = locator.GetStateCollectionName(identity);
            var operatorKey = new BatchOperatorKey()
                .With(nameof(MongoDBStateEntitySaver))
                .With(_connectionName)
                .With(_databaseName)
                .With(_stateCollectionName);
            _batchOperator = (IBatchOperator<StateEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<StateEntity>(options)
                    {
                        DoManyFunc = (entities, cacheData) => SaveManyCoreMany(dbFactory, entities)
                    }));
        }

        private async Task SaveManyCoreMany(
            IDbFactory dbFactory,
            IEnumerable<StateEntity> entities)
        {
            var array = StateEntity.DistinctWithVersion(entities).ToArray();
            var items = array
                .Select(x => new MongoStateEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    version = x.Version,
                    state_data = x.StateData,
                    updated_time = x.UpdatedTime,
                })
                .ToArray();

            var client = dbFactory.GetConnection(_connectionName);
            var db = client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoStateEntity>(_stateCollectionName);

            var upsertModels = items.Select(x =>
            {
                var filter = new ExpressionFilterDefinition<MongoStateEntity>(entity =>
                    entity.claptrap_id == x.claptrap_id && entity.claptrap_type_code == x.claptrap_type_code);
                return new ReplaceOneModel<MongoStateEntity>(filter, x)
                {
                    IsUpsert = true
                };
            });
            await collection.BulkWriteAsync(upsertModels);
        }

        public Task SaveAsync(StateEntity entity)
        {
            return _batchOperator.CreateTask(entity);
        }
    }
}