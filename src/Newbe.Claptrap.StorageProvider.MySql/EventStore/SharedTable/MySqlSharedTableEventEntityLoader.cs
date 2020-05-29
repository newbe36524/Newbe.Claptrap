using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventEntityLoader : IEventEntityLoader<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IMySqlSharedTableEventStoreOptions _options;
        private readonly string _selectSql;

        public MySqlSharedTableEventEntityLoader(
            IDbFactory dbFactory,
            IMySqlSharedTableEventStoreOptions options)
        {
            _dbFactory = dbFactory;
            _options = options;
            _selectSql =
                $"SELECT * FROM {options.SchemaName}.{options.EventTableName} WHERE version >= @startVersion AND version < @endVersion ORDER BY version";
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var dbName = _options.DbName;
            using var db = _dbFactory.GetConnection(dbName);
            var entities = await db.QueryAsync<SharedTableEventEntity>(_selectSql, new {startVersion, endVersion});
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