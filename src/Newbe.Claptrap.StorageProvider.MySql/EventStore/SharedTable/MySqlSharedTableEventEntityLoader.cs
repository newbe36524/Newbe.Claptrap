using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.SharedTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventEntityLoader : IEventEntityLoader<SharedTableEventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly ISqlCache _sqlCache;
        private readonly MySqlDatabaseConfig _mySqlDatabaseConfig;

        public MySqlSharedTableEventEntityLoader(
            IDbFactory dbFactory,
            ISqlCache sqlCache,
            MySqlDatabaseConfig mySqlDatabaseConfig)
        {
            _dbFactory = dbFactory;
            _sqlCache = sqlCache;
            _mySqlDatabaseConfig = mySqlDatabaseConfig;
        }

        public async Task<IEnumerable<SharedTableEventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var dbName = _mySqlDatabaseConfig.SharedTableEventStoreConfig.SharedTableEventStoreDbName;
            using var db = _dbFactory.GetConnection(dbName);
            var sql = _sqlCache.Get(MysqlSqlCacheKeys.SharedTableEventStoreSelectSql);
            var re = await db.QueryAsync<SharedTableEventEntity>(sql, new {startVersion, endVersion});
            return re.ToArray();
        }
    }
}