using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.Extensions;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore
{
    public class PostgreSQLStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly string _insertSql;
        private readonly string _connectionName;

        public PostgreSQLStateEntitySaver(
            IDbFactory dbFactory,
            IClaptrapIdentity identity,
            IRelationalStateStoreLocatorOptions options)
        {
            _dbFactory = dbFactory;
            var locator = options.RelationalStateStoreLocator;
            var (connectionName, schemaName, stateTableName) = locator.GetNames(identity);
            _connectionName = connectionName;
            _insertSql =
                $"INSERT INTO {schemaName}.{stateTableName} (claptrap_type_code,claptrap_id,version,state_data,updated_time) VALUES(@claptrap_type_code, @claptrap_id, @version, @state_data, @updated_time) ON CONFLICT ON CONSTRAINT {stateTableName}_pkey DO UPDATE SET version=@version, state_data=@state_data, updated_time=@updated_time;";
        }

        public async Task SaveAsync(StateEntity entity)
        {
            using var db = _dbFactory.GetConnection(_connectionName);
            var item = new RelationalStateEntity
            {
                claptrap_id = entity.ClaptrapId,
                claptrap_type_code = entity.ClaptrapTypeCode,
                state_data = entity.StateData,
                updated_time = entity.UpdatedTime,
                version = entity.Version,
            };
            await db.ExecuteAsync(_insertSql, item);
        }
    }
}