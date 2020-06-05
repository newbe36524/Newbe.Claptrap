using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Extensions;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore
{
    public class PostgreSQLEventEntityLoader : IEventEntityLoader<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IClaptrapIdentity _masterOrSelfIdentity;
        private readonly string _selectSql;
        private readonly string _connectionName;

        public PostgreSQLEventEntityLoader(
            IDbFactory dbFactory,
            IMasterOrSelfIdentity identity,
            IRelationalEventStoreLocatorOptions options)
        {
            _dbFactory = dbFactory;
            _masterOrSelfIdentity = identity.Identity;
            var (connectionName, schemaName, eventTableName) =
                options.RelationalEventStoreLocator.GetNames(_masterOrSelfIdentity);
            _connectionName = connectionName;
            _selectSql =
                $"SELECT * FROM {schemaName}.{eventTableName} WHERE version >= @startVersion AND version < @endVersion AND claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId ORDER BY version";
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            using var db = _dbFactory.GetConnection(_connectionName);
            var entities = await db.QueryAsync<RelationalEventEntity>(_selectSql, new
                {
                    startVersion, endVersion,
                    ClaptrapTypeCode = _masterOrSelfIdentity.TypeCode,
                    ClaptrapId = _masterOrSelfIdentity.Id
                })
                .ConfigureAwait(false);
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