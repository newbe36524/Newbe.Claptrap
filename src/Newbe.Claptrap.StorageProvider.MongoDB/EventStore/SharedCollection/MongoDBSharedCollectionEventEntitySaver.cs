using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection
{
    public class MongoDBSharedCollectionEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly ISharedCollectionEventBatchSaver _batchSaver;

        public MongoDBSharedCollectionEventEntitySaver(
            IMongoDBSharedCollectionEventStoreOptions options,
            ISharedCollectionEventBatchSaverFactory sharedCollectionEventBatchSaverFactory)
        {
            _batchSaver = sharedCollectionEventBatchSaverFactory.Create(options.ConnectionName,
                options.DatabaseName,
                options.CollectionName);
        }

        public Task SaveAsync(EventEntity entity)
        {
            return _batchSaver.SaveAsync(entity);
        }
    }
}