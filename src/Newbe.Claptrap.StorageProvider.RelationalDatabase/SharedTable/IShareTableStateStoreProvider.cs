namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public interface IShareTableStateStoreProvider
    {
        string CreateUpsertSql(IClaptrapIdentity identity);
        string CreateSelectSql(IClaptrapIdentity identity);
        string SchemaName { get; }
        string StateTableName { get; }
    }
}