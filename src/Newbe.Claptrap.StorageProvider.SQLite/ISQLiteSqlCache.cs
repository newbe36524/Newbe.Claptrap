using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteSqlCache
    {
        void Init();
    }

    public class SQLiteSqlCache : ISQLiteSqlCache
    {
        private readonly ISqlCache _sqlCache;

        public SQLiteSqlCache(
            ISqlCache sqlCache)
        {
            _sqlCache = sqlCache;
        }

        public void Init()
        {
            InitOneIdentityOneTableEventStoreInsertOneSql();
            InitOneIdentityOneTableEventStoreSelectSql();
        }

        private void InitOneIdentityOneTableEventStoreInsertOneSql()
        {
            const string oneIdentityOneTableEventStoreInsertOneSql =
                "INSERT INTO [{eventTableName}] ([version], [eventtypecode], [eventdata], [createdtime]) VALUES (@Version, @EventTypeCode, @EventData, @CreatedTime)";

            _sqlCache.Add(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreInsertOneSql,
                oneIdentityOneTableEventStoreInsertOneSql);
        }

        private void InitOneIdentityOneTableEventStoreSelectSql()
        {
            const string oneIdentityOneTableEventStoreSelectSql =
                "SELECT * FROM [{eventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]";

            _sqlCache.Add(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreSelectSql,
                oneIdentityOneTableEventStoreSelectSql);
        }
    }
}