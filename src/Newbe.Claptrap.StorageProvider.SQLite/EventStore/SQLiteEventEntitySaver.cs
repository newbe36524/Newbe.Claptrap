using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;
using Newbe.ObjectVisitor;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore
{
    public class SQLiteEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly ISQLiteDbFactory _sqLiteDbFactory;
        private readonly ISqlTemplateCache _sqlTemplateCache;
        private readonly IBatchOperator<EventEntity> _batchOperator;
        private readonly string _connectionName;
        private readonly string _eventTableName;

        public SQLiteEventEntitySaver(
            ConcurrentListBatchOperator<EventEntity>.Factory batchOperatorFactory,
            IClaptrapIdentity identity,
            ISQLiteDbFactory sqLiteDbFactory,
            ISQLiteEventStoreOptions options,
            IBatchOperatorContainer batchOperatorContainer,
            ISqlTemplateCache sqlTemplateCache)
        {
            _sqLiteDbFactory = sqLiteDbFactory;
            _sqlTemplateCache = sqlTemplateCache;
            var storeLocator = options.RelationalEventStoreLocator;
            _connectionName = storeLocator.GetConnectionName(identity);
            _eventTableName = storeLocator.GetEventTableName(identity);

            var operatorKey = new BatchOperatorKey()
                .With(nameof(SQLiteEventEntitySaver))
                .With(_connectionName)
                .With(_eventTableName);
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>(options)
                    {
                        DoManyFunc = (entities, cacheData) =>
                            SaveManyAsync(entities),
                        DoManyFuncName = $"event batch saver for {operatorKey.AsStringKey()}"
                    }));
            RegisterSql();
        }

        private int GetCommandHashCode()
        {
            return HashCode.Combine(_connectionName, _eventTableName, 0);
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

        private static readonly ICachedObjectVisitor<RelationalEventEntity, SQLiteCommand> CommandFiller =
            default(RelationalEventEntity)!
                .V()
                .WithExtendObject<RelationalEventEntity, SQLiteCommand>()
                .ForEach((name, value, cmd) => cmd.Parameters.Add(new SQLiteParameter
                {
                    ParameterName = name,
                    Value = value
                }))
                .Cache();

        public async Task SaveManyAsync(IEnumerable<EventEntity> entities)
        {
            var items = entities
                .Select(x => new RelationalEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                })
                .AsParallel();

            var key = GetCommandHashCode();
            var sql = _sqlTemplateCache.GetSql(key);
            var cmd = new SQLiteCommand
            {
                CommandText = sql,
                CommandType = CommandType.Text
            };

            var connection = _sqLiteDbFactory.GetConnection(_connectionName, true);
            var valueTask = connection.BeginTransactionAsync();
            if (!valueTask.IsCompleted)
            {
                await valueTask;
            }

            var transaction = valueTask.Result;
            await using (transaction)
            {
                cmd.Connection = connection;
                foreach (var entity in items)
                {
                    CommandFiller.Run(entity, cmd);
                    await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
        }

        private void RegisterSql()
        {
            var key = GetCommandHashCode();
            _sqlTemplateCache.AddSql(key, () => InitRelationalInsertManySql(_eventTableName));

            static string InitRelationalInsertManySql(
                string eventTableName)
            {
                var propertyNames = typeof(RelationalEventEntity).GetProperties().Select(x => x.Name).ToArray();

                var names = string.Join(",", propertyNames);
                var values = string.Join(",", propertyNames.Select(x => $"@{x}"));
                string sql =
                    $"INSERT INTO {eventTableName} ({names}) VALUES ({values})";
                return sql;
            }
        }
    }
}