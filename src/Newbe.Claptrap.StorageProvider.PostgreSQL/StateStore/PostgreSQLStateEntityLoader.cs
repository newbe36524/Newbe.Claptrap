using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.Extensions;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore
{
    public class PostgreSQLStateEntityLoader
        : IStateEntityLoader<StateEntity>
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IDbFactory _dbFactory;
        private readonly string _selectSql;
        private readonly string _connectionName;

        public PostgreSQLStateEntityLoader(
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IRelationalStateStoreLocatorOptions options)
        {
            _identity = identity;
            _dbFactory = dbFactory;
            var locator = options.RelationalStateStoreLocator;
            var (connectionName, schemaName, stateTableName) = locator.GetNames(identity);
            _connectionName = connectionName;
            _selectSql =
                $"SELECT * FROM {schemaName}.{stateTableName} WHERE claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId LIMIT 1";
        }

        public async Task<StateEntity?> GetStateSnapshotAsync()
        {
            using var db = _dbFactory.GetConnection(_connectionName);
            var ps = new {ClaptrapTypeCode = _identity.TypeCode, ClaptrapId = _identity.Id};
            var item = await db.QueryFirstOrDefaultAsync<RelationalStateEntity>(_selectSql, ps);
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