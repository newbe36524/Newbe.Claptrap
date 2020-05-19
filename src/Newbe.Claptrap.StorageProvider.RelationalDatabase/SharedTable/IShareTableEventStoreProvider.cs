namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public interface IShareTableEventStoreProvider
    {
        string CreateInsertOneSql(IClaptrapIdentity identity);
        string CreateSelectSql(IClaptrapIdentity identity);
        IDbMigrationManager DbMigrationManager(IClaptrapIdentity identity);
    }
}