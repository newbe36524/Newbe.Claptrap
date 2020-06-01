using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileEventEntityLoader
        : IEventEntityLoader<EventEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IDbFactory _dbFactory;
        private readonly string _selectSql;
        private readonly string _connectionName;

        public SQLiteOneIdOneFileEventEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            IDbFactory dbFactory,
            ISQLiteOneIdOneFileEventStoreOptions eventStoreOptions,
            IMasterClaptrapInfo? masterClaptrapInfo = null)
        {
            _claptrapIdentity = claptrapIdentity;
            _dbFactory = dbFactory;
            _selectSql =
                $"SELECT * FROM [{eventStoreOptions.EventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]";
            _connectionName = SQLiteConnectionNameHelper.OneIdOneFileEventStore(
                masterClaptrapInfo?.Identity ?? claptrapIdentity);
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            using var db = _dbFactory.GetConnection(_connectionName);
            var eventEntities =
                await db.QueryAsync<OneIdOneFileEventEntity>(_selectSql, new {startVersion, endVersion});
            var re = eventEntities
                .Select(x => new EventEntity
                {
                    Version = x.version,
                    EventData = x.event_data,
                    EventTypeCode = x.event_type_code,
                    CreatedTime = x.created_time,
                })
                .ToArray();
            foreach (var eventEntity in re)
            {
                eventEntity.ClaptrapId = _claptrapIdentity.Id;
                eventEntity.ClaptrapTypeCode = _claptrapIdentity.TypeCode;
            }

            return re;
        }
    }
}