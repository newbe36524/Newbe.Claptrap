namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public interface IRelationalStateStoreLocator
    {
        string GetConnectionName(IClaptrapIdentity identity);
        string GetSchemaName(IClaptrapIdentity identity);
        string GetStateTableName(IClaptrapIdentity identity);
    }
}