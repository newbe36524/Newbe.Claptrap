using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteSqlCache : ISQLiteSqlCache
    {
        private readonly ISqlTemplateCache _sqlTemplateCache;

        public SQLiteSqlCache(
            ISqlTemplateCache sqlTemplateCache)
        {
            _sqlTemplateCache = sqlTemplateCache;
        }

        public void Init()
        {
            InitOneIdentityOneTableEventStoreInsertOneSql();
            InitOneIdentityOneTableEventStoreSelectSql();
            InitOneIdentityOneTableStateStoreInsertOneSql();
            InitOneIdentityOneTableStateStoreSelectSql();
        }

        private void InitOneIdentityOneTableEventStoreInsertOneSql()
        {
            const string oneIdentityOneTableEventStoreInsertOneSql =
                "INSERT INTO [{0}] ([version], [eventtypecode], [eventdata], [createdtime]) VALUES (@Version, @EventTypeCode, @EventData, @CreatedTime)";

            _sqlTemplateCache.Add(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreInsertOneSql,
                oneIdentityOneTableEventStoreInsertOneSql);
        }

        private void InitOneIdentityOneTableEventStoreSelectSql()
        {
            const string oneIdentityOneTableEventStoreSelectSql =
                "SELECT * FROM [{0}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]";

            _sqlTemplateCache.Add(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreSelectSql,
                oneIdentityOneTableEventStoreSelectSql);
        }

        private void InitOneIdentityOneTableStateStoreInsertOneSql()
        {
            const string oneIdentityOneTableEventStoreInsertOneSql =
                "INSERT OR REPLACE INTO [{0}] ([claptrapid], [claptraptypecode], [version], [statedata],[updatedtime]) VALUES(@ClaptrapId, @ClaptrapTypeCode, @Version, @StateData, @UpdatedTime)";

            _sqlTemplateCache.Add(SQLiteSqlCacheKeys.OneIdentityOneTableStateStoreInsertOneSql,
                oneIdentityOneTableEventStoreInsertOneSql);
        }

        private void InitOneIdentityOneTableStateStoreSelectSql()
        {
            const string oneIdentityOneTableEventStoreSelectSql =
                "SELECT * FROM [{0}] WHERE [claptraptypecode]=@ClaptrapTypeCode AND [claptrapid]=@ClaptrapId LIMIT 1";

            _sqlTemplateCache.Add(SQLiteSqlCacheKeys.OneIdentityOneTableStateStoreSelectSql,
                oneIdentityOneTableEventStoreSelectSql);
        }
    }
}