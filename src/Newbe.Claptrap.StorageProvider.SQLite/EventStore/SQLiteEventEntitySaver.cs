using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Tools;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore
{
    public class SQLiteEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly ISqlTemplateCache _sqlTemplateCache;
        private readonly IBatchOperator<EventEntity> _batchOperator;
        private readonly string _connectionName;
        private readonly string _eventTableName;

        public SQLiteEventEntitySaver(
            BatchOperator<EventEntity>.Factory batchOperatorFactory,
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            ISQLiteEventStoreOptions options,
            IBatchOperatorContainer batchOperatorContainer,
            ISqlTemplateCache sqlTemplateCache)
        {
            _sqlTemplateCache = sqlTemplateCache;
            var storeLocator = options.RelationalEventStoreLocator;
            _connectionName = storeLocator.GetConnectionName(identity);
            _eventTableName = storeLocator.GetEventTableName(identity);
            var operatorKey = new RelationalEventBatchOperatorKey(
                _connectionName,
                _eventTableName);
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>
                    {
                        BufferCount = options.InsertManyWindowCount,
                        BufferTime = options.InsertManyWindowTimeInMilliseconds.HasValue
                            ? TimeSpan.FromMilliseconds(options.InsertManyWindowTimeInMilliseconds.Value)
                            : default,
                        DoManyFunc = entities => SaveManyCoreMany(dbFactory, entities)
                    }));
        }

        private readonly struct RelationalEventBatchOperatorKey : IBatchOperatorKey
        {
            private readonly string _connectionName;
            private readonly string _eventTableName;

            public RelationalEventBatchOperatorKey(
                string connectionName,
                string eventTableName)
            {
                _connectionName = connectionName;
                _eventTableName = eventTableName;
            }

            public string AsStringKey()
            {
                return
                    $"{nameof(SQLiteEventEntitySaver)}-{_connectionName}-{_eventTableName}";
            }
        }

        public Task SaveAsync(EventEntity entity)
        {
            return _batchOperator.CreateTask(entity);
        }

        private async Task SaveManyCoreMany(
            IDbFactory dbFactory,
            IEnumerable<EventEntity> entities)
        {
            var array = entities as EventEntity[] ?? entities.ToArray();
            var items = array
                .Select(x => new RelationalEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                })
                .ToArray();

            var sql = InitRelationalInsertManySql(
                _eventTableName,
                array.Length);
            using var db = dbFactory.GetConnection(_connectionName);
            var ps = new DynamicParameters();
            for (var i = 0; i < array.Length; i++)
            {
                foreach (var (parameterName, valueFunc) in RelationalEventEntity.ValueFactories())
                {
                    var relationalEventEntity = items[i];
                    var name = _sqlTemplateCache.GetParameterName(parameterName, i);
                    ps.Add(name, valueFunc(relationalEventEntity));
                }
            }

            await db.ExecuteAsync(sql, ps);
        }

        private string InitRelationalInsertManySql(
            string eventTableName,
            int count)
        {
            string insertManySqlHeader =
                $"INSERT INTO {eventTableName} (claptrap_type_code, claptrap_id, version, event_type_code, event_data, created_time) VALUES ";
            var valuesSql = Enumerable.Range(0, count)
                .Select(x =>
                    ValuePartFactory(RelationalEventEntity.ParameterNames(), x))
                .ToArray();
            var sb = new StringBuilder(insertManySqlHeader);
            sb.Append(string.Join(",", valuesSql));
            return sb.ToString();
        }

        private string ValuePartFactory(IEnumerable<string> parameters, int index)
        {
            var values = string.Join(",", parameters.Select(x => _sqlTemplateCache.GetParameterName(x, index)));
            var re = $" ({values}) ";
            return re;
        }

        public static void RegisterParameters(ISqlTemplateCache sqlTemplateCache, int maxCount)
        {
            foreach (var name in RelationalEventEntity.ParameterNames())
            {
                for (var i = 0; i < maxCount; i++)
                {
                    sqlTemplateCache.AddParameterName(name, i);
                }
            }
        }
    }
}