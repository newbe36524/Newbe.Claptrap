using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore
{
    /// <summary>
    /// It seem nothing better than <see cref="InsertValuesMySqlEventEntitySaver"/>
    /// </summary>
    public class TransactionMySqlEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IMySqlAdoCache _adoNetCache;
        private readonly IBatchOperator<EventEntity> _batchOperator;
        private readonly string _connectionName;
        private readonly string _eventTableName;

        public TransactionMySqlEventEntitySaver(
            ConcurrentListBatchOperator<EventEntity>.Factory batchOperatorFactory,
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IMySqlEventStoreOptions options,
            IBatchOperatorContainer batchOperatorContainer,
            IMySqlAdoCache adoNetCache)
        {
            _dbFactory = dbFactory;
            _adoNetCache = adoNetCache;
            var storeLocator = options.RelationalEventStoreLocator;
            _connectionName = storeLocator.GetConnectionName(identity);
            _eventTableName = storeLocator.GetEventTableName(identity);

            var operatorKey = new BatchOperatorKey()
                .With(nameof(TransactionMySqlEventEntitySaver))
                .With(_connectionName)
                .With(_eventTableName);
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>(options)
                    {
                        DoManyFunc = (entities, _) =>
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
            var sourceCmd = _adoNetCache.GetCommand(key);
            var connection = _dbFactory.GetConnection(_connectionName);
            await connection.OpenAsync();
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
                    RelationalEventEntity.FillParameter(item, sourceCmd, _adoNetCache, 0);
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
                parameters.Select(x => _adoNetCache.GetParameter(x, index).ParameterName));
            var re = $" ({values}) ";
            return re;
        }

        public static void RegisterParameters(IMySqlAdoCache cache, int maxCount)
        {
            foreach (var name in RelationalEventEntity.ParameterNames())
            {
                for (var i = 0; i < maxCount; i++)
                {
                    cache.AddParameterName(name, i, new MySqlParameter
                    {
                        ParameterName = $"@{name}{i}"
                    });
                }
            }
        }

        private void RegisterSql()
        {
            var key = GetCommandHashCode();
            _adoNetCache.AddCommand(key, () =>
            {
                var cmd = new MySqlCommand
                {
                    CommandText = InitRelationalInsertManySql(_eventTableName),
                    CommandType = CommandType.Text
                };
                foreach (var (parameterName, _) in RelationalEventEntity.ValueFactories)
                {
                    var dataParameter = _adoNetCache.GetParameter(parameterName, 0);
                    cmd.Parameters.Add(dataParameter);
                }

                return cmd;
            });
        }
    }
}