using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore
{
    public class SQLiteEventEntityLoader : IEventEntityLoader<EventEntity>
    {
        private readonly ISQLiteDbFactory _sqLiteDbFactory;
        private readonly IClaptrapIdentity _masterOrSelfIdentity;
        private readonly string _selectSql;
        private readonly string _connectionName;

        public SQLiteEventEntityLoader(
            ISQLiteDbFactory sqLiteDbFactory,
            ISQLiteEventStoreOptions options,
            IMasterOrSelfIdentity masterOrSelfIdentity)
        {
            _sqLiteDbFactory = sqLiteDbFactory;
            _masterOrSelfIdentity = masterOrSelfIdentity.Identity;
            var storeLocator = options.RelationalEventStoreLocator;
            _connectionName = storeLocator.GetConnectionName(_masterOrSelfIdentity);
            var eventTableName = storeLocator.GetEventTableName(_masterOrSelfIdentity);
            _selectSql =
                $"SELECT * FROM {eventTableName} WHERE version >= @startVersion AND version < @endVersion AND claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId ORDER BY version";
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            using var db = _sqLiteDbFactory.GetConnection(_connectionName);
            var entities = await db.QueryAsync<RelationalEventEntity>(_selectSql, new
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
                EventTypeCode = x.event_type_code
            }).ToArray();
            return re;
        }
    }
}