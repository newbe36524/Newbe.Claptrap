namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable
{
    public interface ISharedTableEventBatchSaverFactory
    {
        ISharedTableEventBatchSaver Create(string connectionName,
            string schemaName,
            string eventTableName);
    }
}