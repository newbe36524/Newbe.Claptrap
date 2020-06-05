namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore
{
    public interface IMongoDBStateStoreLocator
    {
        string GetConnectionName(IClaptrapIdentity identity);
        string GetDatabaseName(IClaptrapIdentity identity);
        string GetStateCollectionName(IClaptrapIdentity identity);
    }
}