using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore
{
    public class SQLiteEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly ISQLiteAdoNetCache _sqLiteAdoNetCache;
        private readonly IBatchOperator<EventEntity> _batchOperator;
        private readonly string _connectionName;
        private readonly string _eventTableName;

        public SQLiteEventEntitySaver(
            ConcurrentListBatchOperator<EventEntity>.Factory batchOperatorFactory,
            IClaptrapIdentity identity,
            ISQLiteDbFactory sqLiteDbFactory,
            ISQLiteEventStoreOptions options,
            IBatchOperatorContainer batchOperatorContainer,
            ISQLiteAdoNetCache sqLiteAdoNetCache)
        {
            _sqLiteAdoNetCache = sqLiteAdoNetCache;
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
                            SaveManyCoreMany(sqLiteDbFactory, entities),
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

        private async Task SaveManyCoreMany(ISQLiteDbFactory sqLiteDbFactory,
            IEnumerable<EventEntity> entities)
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
            var sourceCmd = _sqLiteAdoNetCache.GetCommand(key);
            var connection = sqLiteDbFactory.GetConnection(_connectionName, true);
            var valueTask = connection.BeginTransactionAsync();
            if (!valueTask.IsCompleted)
            {
                await valueTask;
            }

            var transaction = valueTask.Result;
            await using (transaction)
            {
                sourceCmd.Connection = connection;
                foreach (var item in items)
                {
                    RelationalEventEntity.FillParameter(item, sourceCmd, _sqLiteAdoNetCache, 0);
                    await sourceCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
        }

        private string InitRelationalInsertManySql(
            string eventTableName)
        {
            string sql =
                $"INSERT INTO {eventTableName} (claptrap_type_code, claptrap_id, version, event_type_code, event_data, created_time) VALUES {ValuePartFactory(RelationalEventEntity.ParameterNames(), 0)}";
            return sql;
        }

        private string ValuePartFactory(IEnumerable<string> parameters, int index)
        {
            var values = string.Join(",",
                parameters.Select(x => _sqLiteAdoNetCache.GetParameter(x, index).ParameterName));
            var re = $" ({values}) ";
            return re;
        }

        public static void RegisterParameters(ISQLiteAdoNetCache cache, int maxCount)
        {
            foreach (var name in RelationalEventEntity.ParameterNames())
            {
                for (var i = 0; i < maxCount; i++)
                {
                    cache.AddParameterName(name, i, new SQLiteParameter
                    {
                        ParameterName = $"@{name}{i}"
                    });
                }
            }
        }

        private void RegisterSql()
        {
            var key = GetCommandHashCode();
            _sqLiteAdoNetCache.AddCommand(key, () =>
            {
                var cmd = new SQLiteCommand
                {
                    CommandText = InitRelationalInsertManySql(_eventTableName),
                    CommandType = CommandType.Text
                };
                foreach (var (parameterName, _) in RelationalEventEntity.ValueFactories)
                {
                    var dataParameter = _sqLiteAdoNetCache.GetParameter(parameterName, 0);
                    cmd.Parameters.Add(dataParameter);
                }

                return cmd;
            });
        }
    }
}