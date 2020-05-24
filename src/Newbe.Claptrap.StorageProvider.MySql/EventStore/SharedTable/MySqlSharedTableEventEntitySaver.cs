using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.SharedTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.Module;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventEntitySaver : BatchEventEntitySaver<SharedTableEventEntity>
    {
        private readonly MySqlDatabaseConfig _mySqlDatabaseConfig;
        private readonly IDbFactory _dbFactory;
        private readonly ISqlCache _sqlCache;

        public MySqlSharedTableEventEntitySaver(
            IBatchEventSaverOptions batchEventSaverOptions,
            MySqlDatabaseConfig mySqlDatabaseConfig,
            IDbFactory dbFactory,
            ISqlCache sqlCache) : base(batchEventSaverOptions)
        {
            _mySqlDatabaseConfig = mySqlDatabaseConfig;
            _dbFactory = dbFactory;
            _sqlCache = sqlCache;
        }

        protected override async Task SaveOneAsync(SharedTableEventEntity entity)
        {
            var sql = _sqlCache.Get(MysqlSqlCacheKeys.SharedTableEventStoreInsertOneSql);
            var dbName = _mySqlDatabaseConfig.SharedTableEventStoreConfig.SharedTableEventStoreDbName;
            using var db = _dbFactory.GetConnection(dbName);
            await db.ExecuteAsync(sql, entity);
        }

        protected override async Task SaveManyAsync(IEnumerable<SharedTableEventEntity> entities)
        {
            var array = entities as SharedTableEventEntity[] ?? entities.ToArray();
            var count = array.Length;
            if (count <= 0)
            {
                return;
            }

            var sql = _sqlCache.Get(MysqlSqlCacheKeys.SharedTableEventStoreInsertManySql(count));
            var dbName = _mySqlDatabaseConfig.SharedTableEventStoreConfig.SharedTableEventStoreDbName;
            using var db = _dbFactory.GetConnection(dbName);
            var ps = new DynamicParameters();
            for (var i = 0; i < count; i++)
            {
                foreach (var (parameterName, valueFunc) in SharedTableEventEntity.ValueFactories())
                {
                    var sharedTableEventEntity = array[i];
                    ps.Add(_sqlCache.GetParameterName(parameterName, i), valueFunc(sharedTableEventEntity));
                }
            }

            await db.ExecuteAsync(sql, ps);
        }
    }
}