using System.Collections.Concurrent;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class SharedTableEventBatchSaverFactory : ISharedTableEventBatchSaverFactory
    {
        private static readonly ConcurrentDictionary<string, SharedTableEventBatchSaver>
            Savers = new ConcurrentDictionary<string, SharedTableEventBatchSaver>();

        private readonly SharedTableEventBatchSaver.Factory _factory;

        public SharedTableEventBatchSaverFactory(
            SharedTableEventBatchSaver.Factory factory)
        {
            _factory = factory;
        }

        public ISharedTableEventBatchSaver Create(string dbName, string schemaName, string eventTableName)
        {
            var sharedTableEventBatchSaver = Savers.GetOrAdd($"{dbName}_{schemaName}_{eventTableName}",
                s => _factory.Invoke(dbName, schemaName, eventTableName));
            return sharedTableEventBatchSaver;
        }
    }
}