using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore
{
    public class MySqlEventEntityLoader : IEventEntityLoader<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly string _selectSql;
        private readonly IClaptrapIdentity _masterOrSelfIdentity;
        private readonly string _connectionName;

        public MySqlEventEntityLoader(
            IDbFactory dbFactory,
            IRelationalEventStoreLocatorOptions options,
            IMasterOrSelfIdentity masterOrSelfIdentity)
        {
            _dbFactory = dbFactory;
            _masterOrSelfIdentity = masterOrSelfIdentity.Identity;
            var locator = options.RelationalEventStoreLocator;
            _connectionName = locator.GetConnectionName(_masterOrSelfIdentity);
            var schemaName = locator.GetSchemaName(_masterOrSelfIdentity);
            var tableName = locator.GetEventTableName(_masterOrSelfIdentity);
            _selectSql =
                $"SELECT * FROM {schemaName}.{tableName} WHERE version >= @startVersion AND version < @endVersion AND claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId ORDER BY version";
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            await using var db = _dbFactory.GetConnection(_connectionName);
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