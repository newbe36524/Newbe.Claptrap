using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Tools;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.SharedTable
{
    public class SQLiteSharedTableEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly IBatchOperator<EventEntity> _batchOperator;

        public SQLiteSharedTableEventEntitySaver(
            BatchOperator<EventEntity>.Factory batchOperatorFactory,
            IDbFactory dbFactory,
            ISQLiteSharedTableEventStoreOptions options,
            IBatchOperatorContainer batchOperatorContainer)
        {
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                new SharedTableEventBatchOperatorKey(options), () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>
                    {
                        BufferCount = options.InsertManyWindowCount,
                        BufferTime = options.InsertManyWindowTimeInMilliseconds.HasValue
                            ? TimeSpan.FromMilliseconds(options.InsertManyWindowTimeInMilliseconds.Value)
                            : default,
                        DoManyFunc = entities => SaveManyCoreMany(dbFactory, options, entities)
                    }));
        }

        private readonly struct SharedTableEventBatchOperatorKey : IBatchOperatorKey
        {
            private readonly ISQLiteSharedTableEventStoreOptions _options;

            public SharedTableEventBatchOperatorKey(
                ISQLiteSharedTableEventStoreOptions options)
            {
                _options = options;
            }

            public string AsStringKey()
            {
                return
                    $"{nameof(SQLiteSharedTableEventEntitySaver)}-{_options.ConnectionName}-{_options.ConnectionName}-{_options.EventTableName}";
            }
        }

        public Task SaveAsync(EventEntity entity)
        {
            return _batchOperator.CreateTask(entity);
        }

        private static async Task SaveManyCoreMany(
            IDbFactory dbFactory,
            ISQLiteSharedTableEventStoreOptions options,
            IEnumerable<EventEntity> entities)
        {
            var array = entities as EventEntity[] ?? entities.ToArray();
            var items = array
                .Select(x => new SharedTableEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                })
                .ToArray();

            var sql = InitSharedTableInsertManySql(
                options.EventTableName,
                array.Length);
            using var db = dbFactory.GetConnection(options.ConnectionName);
            var ps = new DynamicParameters();
            for (var i = 0; i < array.Length; i++)
            {
                foreach (var (parameterName, valueFunc) in SharedTableEventEntity.ValueFactories())
                {
                    var sharedTableEventEntity = items[i];
                    // TODO cache name
                    ps.Add($"@{parameterName}{i}", valueFunc(sharedTableEventEntity));
                }
            }

            await db.ExecuteAsync(sql, ps);
        }

        private static string InitSharedTableInsertManySql(
            string eventTableName,
            int count)
        {
            string insertManySqlHeader =
                $"INSERT INTO {eventTableName} (claptrap_type_code, claptrap_id, version, event_type_code, event_data, created_time) VALUES ";
            var valuesSql = Enumerable.Range(0, count)
                .Select(x =>
                    ValuePartFactory(SharedTableEventEntity.ParameterNames(), x))
                .ToArray();
            var sb = new StringBuilder(insertManySqlHeader);
            sb.Append(string.Join(",", valuesSql));
            return sb.ToString();
        }

        private static string ValuePartFactory(IEnumerable<string> parameters, int index)
        {
            var values = string.Join(",", parameters.Select(x => $"@{x}{index}"));
            var re = $" ({values}) ";
            return re;
        }
    }
}