using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore
{
    public class MongoDBEventEntityLoader : IEventEntityLoader<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IClaptrapIdentity _masterOrSelfIdentity;
        private readonly string _connectionName;
        private readonly string _databaseName;
        private readonly string _eventCollectionName;

        public MongoDBEventEntityLoader(
            IDbFactory dbFactory,
            IMasterOrSelfIdentity masterOrSelfIdentity,
            IMongoDBEventStoreOptions options)
        {
            _dbFactory = dbFactory;
            _masterOrSelfIdentity = masterOrSelfIdentity.Identity;
            var locator = options.MongoDBEventStoreLocator;
            _connectionName = locator.GetConnectionName(_masterOrSelfIdentity);
            _databaseName = locator.GetDatabaseName(_masterOrSelfIdentity);
            _eventCollectionName = locator.GetEventCollectionName(_masterOrSelfIdentity);
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var client = _dbFactory.GetConnection(_connectionName);
            var db = client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoEventEntity>(_eventCollectionName);
            var filter = new ExpressionFilterDefinition<MongoEventEntity>(entity =>
                entity.claptrap_id == _masterOrSelfIdentity.Id
                && entity.claptrap_type_code == _masterOrSelfIdentity.TypeCode
                && entity.version >= startVersion
                && entity.version < endVersion);
            var items = await collection.FindAsync(filter).ConfigureAwait(false);
            var sharedCollectionEventEntities = await items.ToListAsync().ConfigureAwait(false);
            var re = sharedCollectionEventEntities
                .OrderBy(x => x.version)
                .Select(x => new EventEntity
                {
                    Version = x.version,
                    ClaptrapId = x.claptrap_id,
                    CreatedTime = x.created_time,
                    EventData = x.event_data,
                    ClaptrapTypeCode = x.claptrap_type_code,
                    EventTypeCode = x.event_type_code
                });
            return re;
        }
    }
}