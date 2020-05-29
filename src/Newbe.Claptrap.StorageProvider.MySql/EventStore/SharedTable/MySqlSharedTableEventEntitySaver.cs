using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventEntitySaver : BatchEventEntitySaver<EventEntity>
    {
        private readonly IMySqlSharedTableEventStoreOptions _mySqlSharedTableEventStoreOptions;
        private readonly IDbFactory _dbFactory;
        private readonly ISqlTemplateCache _sqlTemplateCache;

        public MySqlSharedTableEventEntitySaver(
            IBatchEventSaverOptions batchEventSaverOptions,
            IMySqlSharedTableEventStoreOptions mySqlSharedTableEventStoreOptions,
            IDbFactory dbFactory,
            ISqlTemplateCache sqlTemplateCache) : base(batchEventSaverOptions)
        {
            _mySqlSharedTableEventStoreOptions = mySqlSharedTableEventStoreOptions;
            _dbFactory = dbFactory;
            _sqlTemplateCache = sqlTemplateCache;
        }

        protected override async Task SaveOneAsync(EventEntity entity)
        {
            var sql = _sqlTemplateCache.Get(MysqlSqlCacheKeys.SharedTableEventStoreInsertOneSql);
            var dbName = _mySqlSharedTableEventStoreOptions.SharedTableEventStoreDbName;
            using var db = _dbFactory.GetConnection(dbName);
            await db.ExecuteAsync(sql, entity);
        }

        protected override async Task SaveManyAsync(IEnumerable<EventEntity> entities)
        {
            var array = entities as EventEntity[] ?? entities.ToArray();
            var count = array.Length;
            if (count <= 0)
            {
                return;
            }

            var sql = _sqlTemplateCache.Get(MysqlSqlCacheKeys.SharedTableEventStoreInsertManySql(count));
            var dbName = _mySqlSharedTableEventStoreOptions.SharedTableEventStoreDbName;
            using var db = _dbFactory.GetConnection(dbName);
            var ps = new DynamicParameters();
            for (var i = 0; i < count; i++)
            {
                foreach (var (parameterName, valueFunc) in EventEntity.ValueFactories())
                {
                    var sharedTableEventEntity = array[i];
                    ps.Add(_sqlTemplateCache.GetParameterName(parameterName, i), valueFunc(sharedTableEventEntity));
                }
            }

            await db.ExecuteAsync(sql, ps);
        }
    }
}