using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore
{
    public class MongoDBStateEntityLoader
        : IStateEntityLoader<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly FilterDefinition<MongoStateEntity> _filter;
        private readonly string _stateCollectionName;
        private readonly string _databaseName;
        private readonly string _connectionName;

        public MongoDBStateEntityLoader(
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

        public async Task<StateEntity?> GetStateSnapshotAsync()
        {
            var client = _dbFactory.GetConnection(_connectionName);
            var db = client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoStateEntity>(_stateCollectionName);
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