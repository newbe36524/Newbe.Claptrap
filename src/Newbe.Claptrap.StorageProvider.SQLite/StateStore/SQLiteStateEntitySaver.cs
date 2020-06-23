using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.Relational.Tools;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore
{
    public class SQLiteStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        public const string UpsertSqlKey = nameof(UpsertSqlKey);
        private readonly ISqlTemplateCache _sqlTemplateCache;
        private readonly IBatchOperator<StateEntity> _batchOperator;
        private readonly string _connectionName;
        private readonly string _stateTableName;

        public SQLiteStateEntitySaver(
            BatchOperator<StateEntity>.Factory batchOperatorFactory,
            IClaptrapIdentity identity,
            ISQLiteDbFactory sqLiteDbFactory,
            ISQLiteStateStoreOptions options,
            ISqlTemplateCache sqlTemplateCache,
            IBatchOperatorContainer batchOperatorContainer)
        {
            _sqlTemplateCache = sqlTemplateCache;
            var locator = options.RelationalStateStoreLocator;
            var stateTableName = locator.GetStateTableName(identity);
            _stateTableName = stateTableName;
            _connectionName = locator.GetConnectionName(identity);
            var operatorKey = new RelationalStateBatchOperatorKey(
                _connectionName,
                stateTableName);
            _batchOperator = (IBatchOperator<StateEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<StateEntity>
                    {
                        // TODO config
                        BufferCount = 100,
                        BufferTime = TimeSpan.FromMilliseconds(50),
                        DoManyFunc = (entities, cacheData) =>
                            SaveManyCoreMany(sqLiteDbFactory, entities, (string[]) cacheData![UpsertSqlKey]),
                        CacheDataFunc = CacheDataFunc
                    }));
        }

        private IReadOnlyDictionary<string, object> CacheDataFunc()
        {
            return new Dictionary<string, object>
            {
                {UpsertSqlKey, InitUpsertSql(_stateTableName)}
            };
        }

        private readonly struct RelationalStateBatchOperatorKey : IBatchOperatorKey
        {
            private readonly string _connectionName;
            private readonly string _stateTableName;

            public RelationalStateBatchOperatorKey(
                string connectionName,
                string stateTableName)
            {
                _connectionName = connectionName;
                _stateTableName = stateTableName;
            }

            public string AsStringKey()
            {
                return
                    $"{nameof(SQLiteStateEntitySaver)}-{_connectionName}-{_stateTableName}";
            }
        }

        private string[] InitUpsertSql(string stateTableName)
        {
            var maxCount = SQLiteEventStoreOptions.SQLiteMaxVariablesCount /
                           RelationalStateEntity.ParameterNames().Count();
            var re = Enumerable.Range(0, maxCount)
                .Select(index => InitRelationalUpsertManySql(stateTableName, index + 1))
                .ToArray();
            return re;
        }

        private async Task SaveManyCoreMany(ISQLiteDbFactory sqLiteDbFactory,
            IEnumerable<StateEntity> entities,
            IReadOnlyList<string> upsertSql)
        {
            var array = entities as StateEntity[] ?? entities.ToArray();
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

            var sql = upsertSql[items.Length - 1];
            using var db = sqLiteDbFactory.GetConnection(_connectionName);
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


        private string InitRelationalUpsertManySql(
            string stateTableName,
            int count)
        {
            string upsertManySqlHeader =
                $"INSERT OR REPLACE INTO {stateTableName} (claptrap_type_code,claptrap_id,version,state_data,updated_time) VALUES ";
            var valuesSql = Enumerable.Range(0, count)
                .Select(x =>
                    ValuePartFactory(RelationalStateEntity.ParameterNames(), x))
                .ToArray();
            var sb = new StringBuilder(upsertManySqlHeader);
            sb.Append(string.Join(",", valuesSql));
            return sb.ToString();
        }

        private string ValuePartFactory(IEnumerable<string> parameters, int index)
        {
            var values = string.Join(",", parameters.Select(x => _sqlTemplateCache.GetParameterName(x, index)));
            var re = $" ({values}) ";
            return re;
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