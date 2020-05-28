using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventEntityLoader : IEventEntityLoader<SharedTableEventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly ISqlTemplateCache _sqlTemplateCache;
        private readonly IMySqlSharedTableEventStoreOptions _mySqlSharedTableEventStoreOptions;

        public MySqlSharedTableEventEntityLoader(
            IDbFactory dbFactory,
            ISqlTemplateCache sqlTemplateCache,
            IMySqlSharedTableEventStoreOptions mySqlSharedTableEventStoreOptions)
        {
            _dbFactory = dbFactory;
            _sqlTemplateCache = sqlTemplateCache;
            _mySqlSharedTableEventStoreOptions = mySqlSharedTableEventStoreOptions;
        }

        public async Task<IEnumerable<SharedTableEventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var dbName = _mySqlSharedTableEventStoreOptions.SharedTableEventStoreDbName;
            using var db = _dbFactory.GetConnection(dbName);
            var sql = _sqlTemplateCache.Get(MysqlSqlCacheKeys.SharedTableEventStoreSelectSql);
            var re = await db.QueryAsync<SharedTableEventEntity>(sql, new {startVersion, endVersion});
            return re.ToArray();
        }
    }
}