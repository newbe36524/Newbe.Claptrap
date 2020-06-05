using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.StateStore
{
    public class MySqlStateEntityLoader
        : IStateEntityLoader<StateEntity>
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IDbFactory _dbFactory;
        private readonly string _selectSql;
        private readonly string _connectionName;

        public MySqlStateEntityLoader(
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IRelationalStateStoreLocatorOptions options)
        {
            var locator = options.RelationalStateStoreLocator;
            _connectionName = locator.GetConnectionName(identity);
            var schemaName = locator.GetSchemaName(identity);
            var eventTableName = locator.GetStateTableName(identity);
            _identity = identity;
            _dbFactory = dbFactory;
            _selectSql =
                $"SELECT * FROM {schemaName}.{eventTableName} WHERE claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId LIMIT 1";
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