namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore
{
    public interface IMongoDBEventStoreLocator
    {
        string GetConnectionName(IClaptrapIdentity identity);
        string GetDatabaseName(IClaptrapIdentity identity);
        string GetEventCollectionName(IClaptrapIdentity identity);
    }
}