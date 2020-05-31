using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore.SharedCollection
{
    public class MongoDBSharedCollectionStateEntityLoader
        : IStateEntityLoader<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IMongoDBSharedCollectionStateStoreOptions _options;
        private readonly FilterDefinition<SharedCollectionStateEntity> _filter;

        public MongoDBSharedCollectionStateEntityLoader(
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

        public async Task<StateEntity?> GetStateSnapshotAsync()
        {
            var client = _dbFactory.GetConnection(_options.ConnectionName);
            var db = client.GetDatabase(_options.DatabaseName);
            var collection = db.GetCollection<SharedCollectionStateEntity>(_options.CollectionName);
            var items = await collection.FindAsync(_filter).ConfigureAwait(false);
            var item = await items.FirstOrDefaultAsync().ConfigureAwait(false);
            if (item == null)
            {
                return null;
            }

            var re = new StateEntity
            {
                Version = item.version,
                ClaptrapId = item.claptrap_id,
                StateData = item.state_data,
                UpdatedTime = item.updated_time,
                ClaptrapTypeCode = item.claptrap_type_code,
            };
            return re;
        }
    }
}