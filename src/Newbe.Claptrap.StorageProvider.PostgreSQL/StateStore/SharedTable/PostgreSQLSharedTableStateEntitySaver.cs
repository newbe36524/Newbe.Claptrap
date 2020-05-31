using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore.SharedTable
{
    public class PostgreSQLSharedTableStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IPostgreSQLSharedTableStateStoreOptions _options;
        private readonly string _insertSql;

        public PostgreSQLSharedTableStateEntitySaver(
            IDbFactory dbFactory,
            IPostgreSQLSharedTableStateStoreOptions options)
        {
            _dbFactory = dbFactory;
            _options = options;
            _insertSql =
                $"INSERT INTO {options.SchemaName}.{options.StateTableName} (claptrap_type_code,claptrap_id,version,state_data,updated_time) VALUES(@claptrap_type_code, @claptrap_id, @version, @state_data, @updated_time) ON CONFLICT ON CONSTRAINT {options.StateTableName}_pkey DO UPDATE SET version=@version, state_data=@state_data, updated_time=@updated_time;";
        }

        public async Task SaveAsync(StateEntity entity)
        {
            using var db = _dbFactory.GetConnection(_options.ConnectionName);
            var item = new SharedTableStateEntity
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