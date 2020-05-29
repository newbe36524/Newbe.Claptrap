namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public interface ISharedTableEventBatchSaverFactory
    {
        ISharedTableEventBatchSaver Create(string dbName,
            string schemaName,
            string eventTableName);
    }
}