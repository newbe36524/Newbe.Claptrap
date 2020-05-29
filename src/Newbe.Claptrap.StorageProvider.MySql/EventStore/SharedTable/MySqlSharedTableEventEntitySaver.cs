using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly ISharedTableEventBatchSaver _batchSaver;

        public MySqlSharedTableEventEntitySaver(
            IMySqlSharedTableEventStoreOptions options,
            ISharedTableEventBatchSaverFactory sharedTableEventBatchSaverFactory)
        {
            _batchSaver = sharedTableEventBatchSaverFactory.Create(options.DbName,
                options.SchemaName,
                options.EventTableName);
        }

        public Task SaveAsync(EventEntity entity)
        {
            return _batchSaver.SaveAsync(entity);
        }
    }
}