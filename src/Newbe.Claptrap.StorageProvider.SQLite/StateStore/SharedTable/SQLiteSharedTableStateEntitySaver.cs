using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.SQLite.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.SharedTable
{
    public class SQLiteSharedTableStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly ISQLiteSharedTableStateStoreOptions _options;
        private readonly string _insertSql;

        public SQLiteSharedTableStateEntitySaver(
            IDbFactory dbFactory,
            ISQLiteSharedTableStateStoreOptions options)
        {
            _dbFactory = dbFactory;
            _options = options;
            _insertSql =
                $"INSERT OR REPLACE INTO {options.StateTableName} (claptrap_type_code,claptrap_id,version,state_data,updated_time) VALUES(@claptrap_type_code, @claptrap_id, @version, @state_data, @updated_time)";
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