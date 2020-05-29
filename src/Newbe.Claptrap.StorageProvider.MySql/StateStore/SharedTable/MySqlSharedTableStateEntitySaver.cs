using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.StateStore.SharedTable
{
    public class MySqlSharedTableStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IMySqlSharedTableStateStoreOptions _options;
        private readonly string _insertSql;

        public MySqlSharedTableStateEntitySaver(
            IDbFactory dbFactory,
            IMySqlSharedTableStateStoreOptions options)
        {
            _dbFactory = dbFactory;
            _options = options;
            _insertSql =
                $"REPLACE INTO {options.SchemaName}.{options.StateTableName} (claptrap_type_code,claptrap_id,version,state_data,updated_time) VALUES(@claptrap_type_code, @claptrap_id, @version, @state_data, @updated_time)";
        }

        public async Task SaveAsync(StateEntity entity)
        {
            using var db = _dbFactory.GetConnection(_options.DbName);
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