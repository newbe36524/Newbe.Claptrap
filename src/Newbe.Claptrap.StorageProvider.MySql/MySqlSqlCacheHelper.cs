using System.Collections.Generic;
using System.Linq;
using Newbe.Claptrap.StorageProvider.MySql.SharedTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlSqlCacheHelper : IMySqlSqlCacheHelper
    {
        private readonly ISqlCache _sqlCache;
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly MySqlDatabaseConfig _mySqlDatabaseConfig;

        public MySqlSqlCacheHelper(
            ISqlCache sqlCache,
            IClaptrapDesignStore claptrapDesignStore,
            MySqlDatabaseConfig mySqlDatabaseConfig)
        {
            _sqlCache = sqlCache;
            _claptrapDesignStore = claptrapDesignStore;
            _mySqlDatabaseConfig = mySqlDatabaseConfig;
        }

        public void Init()
        {
            InitSharedTableInsertOneSql();
            InitSharedTableInsertManySql();
            InitSharedTableEventStoreSelectSql();
        }

        private void InitSharedTableInsertOneSql()
        {
            var config = _mySqlDatabaseConfig.SharedTableEventStoreConfig;
            var sharedTableInsertOneSql =
                $"INSERT INTO [{config.SchemaName}].[{config.EventTableName}] ([claptrap_type_code], [claptrap_id], [version], [event_type_code], [event_data], [created_time]) VALUES (@ClaptrapTypeCode, @ClaptrapId, @Version, @EventTypeCode, @EventData, @CreatedTime)";

            _sqlCache.Add(MysqlSqlCacheKeys.SharedTableEventStoreInsertOneSql, sharedTableInsertOneSql);
        }

        private void InitSharedTableInsertManySql()
        {
            const int maxCount = 10000;
            for (var i = 0; i < maxCount; i++)
            {
                foreach (var name in SharedTableEventEntity.ParameterNames())
                {
                    _sqlCache.AddParameterName(name, i);
                }
            }

            var config = _mySqlDatabaseConfig.SharedTableEventStoreConfig;
            string insertManySqlHeader =
                $"INSERT INTO [{config.SchemaName}].[{config.EventTableName}] ([claptrap_type_code], [claptrap_id], [version], [event_type_code], [event_data], [created_time]) VALUES ";
            var valuesSql = Enumerable.Range(0, maxCount)
                .Select(x =>
                    ValuePartFactory(SharedTableEventEntity.ParameterNames()))
                .ToArray();

            var insertManySql = Enumerable.Range(1, maxCount)
                .ToDictionary(MysqlSqlCacheKeys.SharedTableEventStoreInsertManySql,
                    x => insertManySqlHeader + string.Join(",", valuesSql.Take(x)));
            foreach (var (key, sql) in insertManySql)
            {
                _sqlCache.Add(key, sql);
            }
        }

        private void InitSharedTableEventStoreSelectSql()
        {
            var config = _mySqlDatabaseConfig.SharedTableEventStoreConfig;
            var sql =
                $"SELECT * FROM [{config.SchemaName}].[{config.EventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]";

            _sqlCache.Add(MysqlSqlCacheKeys.SharedTableEventStoreSelectSql, sql);
        }

        private static string ValuePartFactory(IEnumerable<string> parameters)
        {
            var values = string.Join(",", parameters);
            var re = $" ({values}) ";
            return re;
        }
    }
}