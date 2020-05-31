namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public interface ISharedTableEventBatchSaverFactory
    {
        ISharedTableEventBatchSaver Create(string connectionName,
            string schemaName,
            string eventTableName);
    }
}