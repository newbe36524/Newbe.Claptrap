using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore
{
    public class MongoDBStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly FilterDefinition<MongoStateEntity> _filter;
        private readonly string _stateCollectionName;
        private readonly string _databaseName;
        private readonly string _connectionName;

        public MongoDBStateEntitySaver(
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IMongoDBStateStoreLocatorOptions options)
        {
            _dbFactory = dbFactory;
            var locator = options.MongoDBStateStoreLocator;
            _connectionName = locator.GetConnectionName(identity);
            _databaseName = locator.GetDatabaseName(identity);
            _stateCollectionName = locator.GetStateCollectionName(identity);
            _filter = new ExpressionFilterDefinition<MongoStateEntity>(entity =>
                entity.claptrap_id == identity.Id
                && entity.claptrap_type_code == identity.TypeCode);
        }

        public async Task SaveAsync(StateEntity entity)
        {
            var item = new MongoStateEntity
            {
                claptrap_id = entity.ClaptrapId,
                claptrap_type_code = entity.ClaptrapTypeCode,
                state_data = entity.StateData,
                updated_time = entity.UpdatedTime,
                version = entity.Version,
            };

            var client = _dbFactory.GetConnection(_connectionName);
            var db = client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoStateEntity>(_stateCollectionName);
            var updateOptions = new ReplaceOptions
            {
                IsUpsert = true,
            };
            await collection.ReplaceOneAsync(_filter, item, updateOptions)
                .ConfigureAwait(false);
        }
    }
}