namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public interface IRelationalEventStoreLocator
    {
        string GetConnectionName(IClaptrapIdentity identity);
        string GetSchemaName(IClaptrapIdentity identity);
        string GetEventTableName(IClaptrapIdentity identity);
    }
}