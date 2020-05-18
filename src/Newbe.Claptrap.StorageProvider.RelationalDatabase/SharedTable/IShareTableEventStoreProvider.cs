namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public interface IShareTableEventStoreProvider
    {
        string CreateInsertOneSql(IClaptrapIdentity identity);
        string CreateSelectSql(IClaptrapIdentity identity);
        string SchemaName { get; }
        string EventTableName { get; }
    }
}