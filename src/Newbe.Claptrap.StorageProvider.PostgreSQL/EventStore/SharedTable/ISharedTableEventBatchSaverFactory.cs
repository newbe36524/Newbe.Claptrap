namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable
{
    public interface ISharedTableEventBatchSaverFactory
    {
        ISharedTableEventBatchSaver Create(string dbName,
            string schemaName,
            string eventTableName);
    }
}