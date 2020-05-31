using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection
{
    public class SharedCollectionEventBatchSaverFactory : ISharedCollectionEventBatchSaverFactory
    {
        private readonly Dictionary<string, SharedCollectionEventBatchSaver>
            _savers = new Dictionary<string, SharedCollectionEventBatchSaver>();

        private readonly object _locker = new object();
        private readonly SharedCollectionEventBatchSaver.Factory _factory;

        public SharedCollectionEventBatchSaverFactory(
            SharedCollectionEventBatchSaver.Factory factory)
        {
            _factory = factory;
        }

        public ISharedCollectionEventBatchSaver Create(string connectionName, string schemaName, string eventTableName)
        {
            var key = $"{connectionName}_{schemaName}_{eventTableName}";
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

                saver = _factory.Invoke(connectionName, schemaName, eventTableName);
                _savers[key] = saver;
                return saver;
            }
        }
    }
}