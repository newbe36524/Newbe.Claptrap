using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational.Extensions;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore
{
    public class PostgreSQLStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        public const string UpsertSqlKey = nameof(UpsertSqlKey);
        private readonly IBatchOperator<StateEntity> _batchOperator;
        private readonly string _connectionName;
        private readonly string _schemaName;
        private readonly string _stateTableName;

        public PostgreSQLStateEntitySaver(
            BatchOperator<StateEntity>.Factory batchOperatorFactory,
            IBatchOperatorContainer batchOperatorContainer,
            IDbFactory dbFactory,
            IClaptrapIdentity identity,
            IPostgreSQLStateStoreOptions options)
        {
            var locator = options.RelationalStateStoreLocator;
            var (connectionName, schemaName, stateTableName) = locator.GetNames(identity);
            _connectionName = connectionName;
            _schemaName = schemaName;
            _stateTableName = stateTableName;

            var operatorKey = new BatchOperatorKey()
                .With(nameof(PostgreSQLStateEntitySaver))
                .With(_connectionName)
                .With(_schemaName)
                .With(_stateTableName);

            _batchOperator = (IBatchOperator<StateEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<StateEntity>(options)
                    {
                        DoManyFunc = (entities, cacheData) =>
                            SaveManyCoreMany(dbFactory, entities, (string) cacheData![UpsertSqlKey]),
                        CacheDataFunc = CacheDataFunc
                    }));
        }

        private IReadOnlyDictionary<string, object> CacheDataFunc()
        {
            return new Dictionary<string, object>
            {
                {
                    UpsertSqlKey,
                    $"INSERT INTO {_schemaName}.{_stateTableName} (claptrap_type_code, claptrap_id, version, state_data, updated_time) VALUES (unnest(@claptrap_type_code), unnest(@claptrap_id), unnest(@version), unnest(@state_data), unnest(@updated_time)) ON CONFLICT ON CONSTRAINT {_stateTableName}_pkey DO UPDATE SET version = excluded.version, state_data = excluded.state_data, updated_time = excluded.updated_time WHERE excluded.version > {_schemaName}.{_stateTableName}.version;"
                }
            };
        }

        private async Task SaveManyCoreMany(IDbFactory factory, IEnumerable<StateEntity> entities, string upsertSql)
        {
            var items = StateEntity.DistinctWithVersion(entities).ToArray();

            var data = new
            {
                claptrap_id = items.Select(x => x.ClaptrapId).ToArray(),
                claptrap_type_code = items.Select(x => x.ClaptrapTypeCode).ToArray(),
                version = items.Select(x => x.Version).ToArray(),
                state_data = items.Select(x => x.StateData).ToArray(),
                updated_time = items.Select(x => x.UpdatedTime).ToArray()
            };
            using var db = factory.GetConnection(_connectionName);
            await db.ExecuteAsync(upsertSql, data);
        }

        public Task SaveAsync(StateEntity entity)
        {
            return _batchOperator.CreateTask(entity).AsTask();
        }
    }
}