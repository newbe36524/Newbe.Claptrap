namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public static class SQLiteSqlCacheKeys
    {
        private const int Prefix = 2_000_000;
        private const int SharedTableEventStorePrefix = Prefix + 100_000;
        public const int SharedTableEventStoreInsertOneSql = SharedTableEventStorePrefix + 10_000;

        public static int SharedTableEventStoreInsertManySql(int count)
        {
            return SharedTableEventStorePrefix + 20_000 + count;
        }

        public const int SharedTableEventStoreSelectSql = SharedTableEventStorePrefix + 30_000;

        private const int OneTypeOneTableEventStorePrefix = Prefix + 200_000;
        private const int OneIdentityOneTableEventStorePrefix = Prefix + 300_000;
        public const int OneIdentityOneTableEventStoreInsertOneSql = OneIdentityOneTableEventStorePrefix + 10_000;

        public const int OneIdentityOneTableEventStoreSelectSql = OneIdentityOneTableEventStorePrefix + 30_000;


        private const int OneTypeOneTableStateStorePrefix = Prefix + 600_000;

        public const int OneIdentityOneTableStateStoreInsertOneSql = OneTypeOneTableStateStorePrefix + 10_000;

        public const int OneIdentityOneTableStateStoreSelectSql = OneTypeOneTableStateStorePrefix + 30_000;
    }
}