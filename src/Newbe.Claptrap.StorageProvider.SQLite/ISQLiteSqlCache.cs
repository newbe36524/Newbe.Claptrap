using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteSqlCache
    {
        void Init();
    }

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
    }
}