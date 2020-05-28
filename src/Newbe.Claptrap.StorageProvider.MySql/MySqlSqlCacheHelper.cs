using System.Collections.Generic;
using System.Linq;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlSqlCacheHelper : IMySqlSqlCacheHelper
    {
        private readonly ISqlTemplateCache _sqlTemplateCache;
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly IMySqlSharedTableEventStoreOptions? _mySqlSharedTableEventStoreOptions;

        public MySqlSqlCacheHelper(
            ISqlTemplateCache sqlTemplateCache,
            IClaptrapDesignStore claptrapDesignStore,
            IMySqlSharedTableEventStoreOptions? mySqlSharedTableEventStoreOptions = null)
        {
            _sqlTemplateCache = sqlTemplateCache;
            _claptrapDesignStore = claptrapDesignStore;
            _mySqlSharedTableEventStoreOptions = mySqlSharedTableEventStoreOptions;
        }

        public void Init()
        {
            if (_mySqlSharedTableEventStoreOptions != null)
            {
                InitSharedTableInsertOneSql();
                InitSharedTableInsertManySql();
                InitSharedTableEventStoreSelectSql();
            }
        }

        private void InitSharedTableInsertOneSql()
        {
            var config = _mySqlSharedTableEventStoreOptions;
            var sharedTableInsertOneSql =
                $"INSERT INTO [{config.SchemaName}].[{config.EventTableName}] ([claptrap_type_code], [claptrap_id], [version], [event_type_code], [event_data], [created_time]) VALUES (@ClaptrapTypeCode, @ClaptrapId, @Version, @EventTypeCode, @EventData, @CreatedTime)";

            _sqlTemplateCache.Add(MysqlSqlCacheKeys.SharedTableEventStoreInsertOneSql, sharedTableInsertOneSql);
        }

        private void InitSharedTableInsertManySql()
        {
            const int maxCount = 10000;
            for (var i = 0; i < maxCount; i++)
            {
                foreach (var name in SharedTableEventEntity.ParameterNames())
                {
                    _sqlTemplateCache.AddParameterName(name, i);
                }
            }

            var config = _mySqlSharedTableEventStoreOptions;
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
                _sqlTemplateCache.Add(key, sql);
            }
        }

        private void InitSharedTableEventStoreSelectSql()
        {
            var config = _mySqlSharedTableEventStoreOptions;
            var sql =
                $"SELECT * FROM [{config.SchemaName}].[{config.EventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]";

            _sqlTemplateCache.Add(MysqlSqlCacheKeys.SharedTableEventStoreSelectSql, sql);
        }

        private static string ValuePartFactory(IEnumerable<string> parameters)
        {
            var values = string.Join(",", parameters);
            var re = $" ({values}) ";
            return re;
        }
    }
}