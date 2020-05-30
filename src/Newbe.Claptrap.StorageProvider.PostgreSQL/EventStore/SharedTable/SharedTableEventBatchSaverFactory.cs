using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable
{
    public class SharedTableEventBatchSaverFactory : ISharedTableEventBatchSaverFactory
    {
        private readonly Dictionary<string, SharedTableEventBatchSaver>
            _savers = new Dictionary<string, SharedTableEventBatchSaver>();

        private readonly object _locker = new object();
        private readonly SharedTableEventBatchSaver.Factory _factory;

        public SharedTableEventBatchSaverFactory(
            SharedTableEventBatchSaver.Factory factory)
        {
            _factory = factory;
        }

        public ISharedTableEventBatchSaver Create(string dbName, string schemaName, string eventTableName)
        {
            var key = $"{dbName}_{schemaName}_{eventTableName}";
            if (_savers.TryGetValue(key, out var saver))
            {
                return saver;
            }

            lock (_locker)
            {
                if (_savers.TryGetValue(key, out saver))
                {
                    return saver;
                }

                saver = _factory.Invoke(dbName, schemaName, eventTableName);
                _savers[key] = saver;
                return saver;
            }
        }
    }
}