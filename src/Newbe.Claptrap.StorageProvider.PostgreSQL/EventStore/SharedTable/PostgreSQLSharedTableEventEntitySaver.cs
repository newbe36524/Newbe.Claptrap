using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable
{
    public class PostgreSQLSharedTableEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly ISharedTableEventBatchSaver _batchSaver;

        public PostgreSQLSharedTableEventEntitySaver(
            IPostgreSQLSharedTableEventStoreOptions options,
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