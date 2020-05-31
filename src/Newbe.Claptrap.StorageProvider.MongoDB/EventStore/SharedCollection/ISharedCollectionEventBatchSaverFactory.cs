namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection
{
    public interface ISharedCollectionEventBatchSaverFactory
    {
        ISharedCollectionEventBatchSaver Create(string connectionName,
            string schemaName,
            string eventTableName);
    }
}