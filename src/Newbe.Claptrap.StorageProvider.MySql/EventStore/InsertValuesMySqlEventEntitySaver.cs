using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.ObjectVisitor;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore
{
    public class InsertValuesMySqlEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly ISqlTemplateCache _sqlTemplateCache;
        private readonly IBatchOperator<EventEntity> _batchOperator;
        private readonly string _eventTableName;
        private readonly string _connectionName;
        private readonly string _schemaName;

        public InsertValuesMySqlEventEntitySaver(
            ConcurrentListBatchOperator<EventEntity>.Factory batchOperatorFactory,
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IMySqlEventStoreOptions options,
            IBatchOperatorContainer batchOperatorContainer,
            ISqlTemplateCache sqlTemplateCache)
        {
            var locator = options.RelationalEventStoreLocator;
            _connectionName = locator.GetConnectionName(identity);
            _schemaName = locator.GetSchemaName(identity);
            _eventTableName = locator.GetEventTableName(identity);
            _dbFactory = dbFactory;
            _sqlTemplateCache = sqlTemplateCache;
            var operatorKey = new BatchOperatorKey()
                .With(nameof(InsertValuesMySqlEventEntitySaver))
                .With(_connectionName)
                .With(_schemaName)
                .With(_eventTableName);
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>(options)
                    {
                        DoManyFunc = (entities, cacheData) => SaveManyAsync(entities),
                        DoManyFuncName = $"event batch saver for {operatorKey.AsStringKey()}"
                    }));
            RegisterSql();
        }

        private int GetCommandHashCode(int count)
        {
            return HashCode.Combine(_connectionName, _schemaName, _eventTableName, count);
        }

        private void RegisterSql()
        {
            for (var i = 0; i < 1000; i++)
            {
                var count = i + 1;
                var key = GetCommandHashCode(count);
                _sqlTemplateCache.AddSql(key, () =>
                {
                    var sql = InitRelationalInsertManySql(_schemaName, _eventTableName, count);
                    return sql;
                });
            }
        }

        public Task SaveAsync(EventEntity entity)
        {
            var valueTask = _batchOperator.CreateTask(entity);
            if (valueTask.IsCompleted)
            {
                return Task.CompletedTask;
            }

            return valueTask.AsTask();
        }

        private static readonly
            ICachedObjectVisitor<RelationalEventEntity, (ISqlTemplateCache cache, MySqlCommand cmd, int index)>
            CommandFiller = default(RelationalEventEntity)!
                .V()
                .WithExtendObject<RelationalEventEntity, (ISqlTemplateCache cache, MySqlCommand cmd, int index)>()
                .ForEach((name, value, data) =>
                    data.cmd.Parameters.Add(new MySqlParameter(data.cache.GetParameterName(name, data.index), value)))
                .Cache();

        public async Task SaveManyAsync(IEnumerable<EventEntity> entities)
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

            var sql = _sqlTemplateCache.GetSql(GetCommandHashCode(array.Length));
            await using var db = _dbFactory.GetConnection(_connectionName);
            var cmd = new MySqlCommand(sql, db);
            for (var i = 0; i < array.Length; i++)
            {
                var eventEntity = items[i];
                CommandFiller.Run(eventEntity, (_sqlTemplateCache, cmd, i));
            }

            await db.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        private string InitRelationalInsertManySql(
            string schemaName,
            string eventTableName,
            int count)
        {
            var names = typeof(RelationalEventEntity).GetProperties().Select(x=>x.Name).ToArray();
            string insertManySqlHeader =
                $"INSERT INTO {schemaName}.{eventTableName} (claptrap_type_code, claptrap_id, version, event_type_code, event_data, created_time) VALUES ";
            var valuesSql = Enumerable.Range(0, count)
                .Select(x =>
                    ValuePartFactory(names, x))
                .ToArray();
            var sb = new StringBuilder(insertManySqlHeader);
            sb.Append(string.Join(",", valuesSql));
            return sb.ToString();
        }

        private string ValuePartFactory(IEnumerable<string> parameters, int index)
        {
            var values = string.Join(",", parameters.Select(x =>
                _sqlTemplateCache.GetParameterName(x, index)));
            var re = $" ({values}) ";
            return re;
        }

        public static void RegisterParameters(ISqlTemplateCache sqlTemplateCache, int maxCount)
        {
            var names = typeof(RelationalEventEntity).GetProperties().Select(x=>x.Name).ToArray();
            foreach (var name in names)
            {
                for (var i = 0; i < maxCount; i++)
                {
                    sqlTemplateCache.AddParameterName(name, i);
                }
            }
        }
    }
}