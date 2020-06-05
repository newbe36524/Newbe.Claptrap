using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore
{
    public class SQLiteStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly string _insertSql;
        private readonly string _connectionName;

        public SQLiteStateEntitySaver(
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            ISQLiteStateStoreOptions options)
        {
            _dbFactory = dbFactory;
            var locator = options.RelationalStateStoreLocator;
            var stateTableName = locator.GetStateTableName(identity);
            _connectionName = locator.GetConnectionName(identity);
            _insertSql =
                $"INSERT OR REPLACE INTO {stateTableName} (claptrap_type_code,claptrap_id,version,state_data,updated_time) VALUES(@claptrap_type_code, @claptrap_id, @version, @state_data, @updated_time)";
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