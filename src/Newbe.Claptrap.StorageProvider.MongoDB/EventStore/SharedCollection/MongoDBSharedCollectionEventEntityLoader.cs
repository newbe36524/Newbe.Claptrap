using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection
{
    public class MongoDBSharedCollectionEventEntityLoader : IEventEntityLoader<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IMongoDBSharedCollectionEventStoreOptions _options;
        private readonly IClaptrapIdentity _masterOrSelfIdentity;

        public MongoDBSharedCollectionEventEntityLoader(
            IDbFactory dbFactory,
            IClaptrapIdentity identity,
            IMongoDBSharedCollectionEventStoreOptions options,
            IMasterClaptrapInfo masterClaptrapInfo = null)
        {
            _dbFactory = dbFactory;
            _options = options;
            _masterOrSelfIdentity = masterClaptrapInfo?.Identity ?? identity;
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var client = _dbFactory.GetConnection(_options.ConnectionName);
            var db = client.GetDatabase(_options.DatabaseName);
            var collection = db.GetCollection<SharedCollectionEventEntity>(_options.CollectionName);
            var filter = new ExpressionFilterDefinition<SharedCollectionEventEntity>(entity =>
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
                    EventTypeCode = x.event_type_code,
                });
            return re;
        }
    }
}