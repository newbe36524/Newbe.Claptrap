using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.StateStore
{
    public class MySqlStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly ISqlTemplateCache _sqlTemplateCache;
        private readonly string _connectionName;
        private readonly IBatchOperator<StateEntity> _batchOperator;
        private readonly string _tableName;
        private readonly string _schemaName;

        private readonly ConcurrentDictionary<int, string> _upsertSqlCache =
            new ConcurrentDictionary<int, string>();

        public MySqlStateEntitySaver(
            BatchOperator<StateEntity>.Factory batchOperatorFactory,
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IRelationalStateStoreLocatorOptions options,
            ISqlTemplateCache sqlTemplateCache,
            IBatchOperatorContainer batchOperatorContainer)
        {
            var locator = options.RelationalStateStoreLocator;
            _connectionName = locator.GetConnectionName(identity);
            _schemaName = locator.GetSchemaName(identity);
            _tableName = locator.GetStateTableName(identity);
            _sqlTemplateCache = sqlTemplateCache;

            var operatorKey = new BatchOperatorKey()
                .With(nameof(MySqlStateEntitySaver))
                .With(_connectionName)
                .With(_tableName);
            _batchOperator = (IBatchOperator<StateEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<StateEntity>(options)
                    {
                        DoManyFunc = (entities, cacheData) =>
                            SaveManyCoreMany(dbFactory, entities)
                    }));
        }

        private async Task SaveManyCoreMany(IDbFactory dbFactory,
            IEnumerable<StateEntity> entities)
        {
            var array = StateEntity.DistinctWithVersion(entities).ToArray();
            var items = array
                .Select(x => new RelationalStateEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    version = x.Version,
                    state_data = x.StateData,
                    updated_time = x.UpdatedTime
                })
                .AsParallel()
                .ToArray();

            var sql = GetUpsertSql(_schemaName, _tableName, items.Length);
            using var db = dbFactory.GetConnection(_connectionName);
            var ps = new DynamicParameters();
            for (var i = 0; i < array.Length; i++)
            {
                foreach (var (parameterName, valueFunc) in RelationalStateEntity.ValueFactories())
                {
                    var entity = items[i];
                    var name = _sqlTemplateCache.GetParameterName(parameterName, i);
                    ps.Add(name, valueFunc(entity));
                }
            }

            await db.ExecuteAsync(sql, ps);
        }

        private string GetUpsertSql(string schemaName, string tableName, int count)
        {
            var sql = _upsertSqlCache.GetOrAdd(count, CreateUpsertSql);
            return sql;


            string CreateUpsertSql(int valueCount)
            {
                string upsertManySqlHeader =
                    $"REPLACE INTO {schemaName}.{tableName} (claptrap_type_code,claptrap_id,version,state_data,updated_time) VALUES ";
                var sb = new StringBuilder(upsertManySqlHeader);
                var valuesSql = Enumerable.Range(0, count)
                    .Select(x =>
                        ValuePartFactory(RelationalStateEntity.ParameterNames(), x))
                    .ToArray();
                sb.Append(string.Join(",", valuesSql));
                return sb.ToString();

                string ValuePartFactory(IEnumerable<string> parameters, int index)
                {
                    var values = string.Join(",", parameters.Select(x => _sqlTemplateCache.GetParameterName(x, index)));
                    var re = $" ({values}) ";
                    return re;
                }
            }
        }

        public Task SaveAsync(StateEntity entity)
        {
            return _batchOperator.CreateTask(entity);
        }

        public static void RegisterParameters(ISqlTemplateCache sqlTemplateCache, int maxCount)
        {
            foreach (var name in RelationalStateEntity.ParameterNames())
            {
                for (var i = 0; i < maxCount; i++)
                {
                    sqlTemplateCache.AddParameterName(name, i);
                }
            }
        }
    }
}