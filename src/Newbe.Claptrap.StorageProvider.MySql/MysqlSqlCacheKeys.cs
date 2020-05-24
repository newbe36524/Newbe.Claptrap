namespace Newbe.Claptrap.StorageProvider.MySql
{
    public static class MysqlSqlCacheKeys
    {
        private const int Prefix = 1_000_000;
        private const int SharedTableEventStorePrefix = Prefix + 100_000;
        public const int SharedTableEventStoreInsertOneSql = SharedTableEventStorePrefix + 10_000;

        public static int SharedTableEventStoreInsertManySql(int count)
        {
            return SharedTableEventStorePrefix + 20_000 + count;
        }

        public const int SharedTableEventStoreSelectSql = SharedTableEventStorePrefix + 30_000;
    }
}