using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventEntitySaver : BatchEventEntitySaver<EventEntity>
    {
        private readonly IMySqlSharedTableEventStoreOptions _options;
        private readonly IDbFactory _dbFactory;
        private readonly string _insertOneSql;
        private readonly ISharedTableEventBatchSaver _batchSaver;

        public MySqlSharedTableEventEntitySaver(
            IBatchEventSaverOptions batchEventSaverOptions,
            IMySqlSharedTableEventStoreOptions options,
            ISharedTableEventBatchSaverFactory sharedTableEventBatchSaverFactory,
            IDbFactory dbFactory) : base(batchEventSaverOptions)
        {
            _options = options;
            _dbFactory = dbFactory;
            _batchSaver = sharedTableEventBatchSaverFactory.Create(options.DbName,
                options.SchemaName,
                options.EventTableName);
            _insertOneSql =
                $"INSERT INTO {options.SchemaName}.{options.EventTableName} (claptrap_type_code, claptrap_id, version, event_type_code, event_data, created_time) VALUES (@ClaptrapTypeCode, @ClaptrapId, @Version, @EventTypeCode, @EventData, @CreatedTime)";
        }

        protected override async Task SaveOneAsync(EventEntity entity)
        {
            var dbName = _options.DbName;
            using var db = _dbFactory.GetConnection(dbName);
            await db.ExecuteAsync(_insertOneSql, entity);
        }

        protected override Task SaveManyAsync(IEnumerable<EventEntity> entities)
        {
            var array = entities as EventEntity[] ?? entities.ToArray();
            var count = array.Length;
            if (count <= 0)
            {
                return Task.CompletedTask;
            }

            return _batchSaver.SaveManyAsync(array);
        }
    }
}