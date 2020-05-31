using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore.SharedCollection
{
    public class MongoDBSharedCollectionStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IMongoDBSharedCollectionStateStoreOptions _options;
        private readonly FilterDefinition<SharedCollectionStateEntity> _filter;

        public MongoDBSharedCollectionStateEntitySaver(
            IClaptrapIdentity claptrapIdentity,
            IDbFactory dbFactory,
            IMongoDBSharedCollectionStateStoreOptions options)
        {
            _dbFactory = dbFactory;
            _options = options;
            _filter = new ExpressionFilterDefinition<SharedCollectionStateEntity>(entity =>
                entity.claptrap_id == claptrapIdentity.Id
                && entity.claptrap_type_code == claptrapIdentity.TypeCode);
        }

        public async Task SaveAsync(StateEntity entity)
        {
            var item = new SharedCollectionStateEntity
            {
                claptrap_id = entity.ClaptrapId,
                claptrap_type_code = entity.ClaptrapTypeCode,
                state_data = entity.StateData,
                updated_time = entity.UpdatedTime,
                version = entity.Version,
            };

            var client = _dbFactory.GetConnection(_options.ConnectionName);
            var db = client.GetDatabase(_options.DatabaseName);
            var collection = db.GetCollection<SharedCollectionStateEntity>(_options.CollectionName);
            var updateOptions = new ReplaceOptions
            {
                IsUpsert = true,
            };
            await collection.ReplaceOneAsync(_filter, item, updateOptions)
                .ConfigureAwait(false);
        }
    }
}