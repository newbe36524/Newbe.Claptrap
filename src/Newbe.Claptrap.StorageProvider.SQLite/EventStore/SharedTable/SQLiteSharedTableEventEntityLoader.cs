using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.SharedTable
{
    public class SQLiteSharedTableEventEntityLoader : IEventEntityLoader<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly ISQLiteSharedTableEventStoreOptions _options;
        private readonly string _selectSql;
        private readonly IClaptrapIdentity _masterOrSelfIdentity;

        public SQLiteSharedTableEventEntityLoader(
            IDbFactory dbFactory,
            IClaptrapIdentity identity,
            ISQLiteSharedTableEventStoreOptions options,
            IMasterClaptrapInfo? masterClaptrapInfo = null)
        {
            _dbFactory = dbFactory;
            _options = options;
            _masterOrSelfIdentity = masterClaptrapInfo?.Identity ?? identity;
            _selectSql =
                $"SELECT * FROM {options.EventTableName} WHERE version >= @startVersion AND version < @endVersion AND claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId ORDER BY version";
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var connectionName = _options.ConnectionName;
            using var db = _dbFactory.GetConnection(connectionName);
            var entities = await db.QueryAsync<SharedTableEventEntity>(_selectSql, new
            {
                startVersion,
                endVersion,
                ClaptrapTypeCode = _masterOrSelfIdentity.TypeCode,
                ClaptrapId = _masterOrSelfIdentity.Id
            });
            var re = entities.Select(x => new EventEntity
            {
                Version = x.version,
                ClaptrapId = x.claptrap_id,
                CreatedTime = x.created_time,
                EventData = x.event_data,
                ClaptrapTypeCode = x.claptrap_type_code,
                EventTypeCode = x.event_type_code,
            }).ToArray();
            return re;
        }
    }
}