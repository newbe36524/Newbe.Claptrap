using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.StateStore
{
    public class MySqlStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly string _insertSql;
        private readonly string _connectionName;

        public MySqlStateEntitySaver(
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IRelationalStateStoreLocatorOptions options)
        {
            var locator = options.RelationalStateStoreLocator;
            _connectionName = locator.GetConnectionName(identity);
            var schemaName = locator.GetSchemaName(identity);
            var eventTableName = locator.GetStateTableName(identity);
            _dbFactory = dbFactory;
            _insertSql =
                $"REPLACE INTO {schemaName}.{eventTableName} (claptrap_type_code,claptrap_id,version,state_data,updated_time) VALUES(@claptrap_type_code, @claptrap_id, @version, @state_data, @updated_time)";
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