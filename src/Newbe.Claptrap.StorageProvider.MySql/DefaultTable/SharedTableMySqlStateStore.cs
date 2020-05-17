using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.MySql.DefaultTable
{
    public class SharedTableMySqlStateStore : IStateLoader, IStateSaver
    {
        private readonly SharedStateTableDef.Factory _factory;
        private readonly IMySqlDbFactory _mySqlDbFactory;
        private readonly IClock _clock;
        private readonly IStateDataStringSerializer _stateDataStringSerializer;
        private readonly ILogger<SharedTableMySqlStateStore> _logger;
        private readonly Lazy<string> _selectSql;
        private readonly Lazy<string> _upsertSql;

        public delegate SharedTableMySqlStateStore Factory(IClaptrapIdentity identity);

        public SharedTableMySqlStateStore(IClaptrapIdentity identity,
            SharedStateTableDef.Factory factory,
            IMySqlDbFactory mySqlDbFactory,
            IClock clock,
            IStateDataStringSerializer stateDataStringSerializer,
            ILogger<SharedTableMySqlStateStore> logger)
        {
            _factory = factory;
            _mySqlDbFactory = mySqlDbFactory;
            _clock = clock;
            _stateDataStringSerializer = stateDataStringSerializer;
            _logger = logger;
            Identity = identity;
            _selectSql = new Lazy<string>(() =>
            {
                var def = _factory.Invoke();
                return
                    $"SELECT * FROM [{def.SchemaName}].[{def.StateTableName}] WHERE [claptrap_type_code]=@ClaptrapTypeCode AND [claptrap_id]=@ClaptrapId LIMIT 1";
            });
            _upsertSql = new Lazy<string>(() =>
            {
                var def = _factory.Invoke();
                return
                    $"REPLACE INTO [{def.SchemaName}].[{def.StateTableName}] ([claptrap_type_code],[claptrap_id],[version],[state_data],[updated_time]) VALUES(@ClaptrapTypeCode, @ClaptrapId, @Version, @StateData, @UpdatedTime)";
            });
        }

        public IClaptrapIdentity Identity { get; }

        public async Task SaveAsync(IState state)
        {
            var sql = _upsertSql.Value;
            var stateData = _stateDataStringSerializer.Serialize(state.Identity.TypeCode, state.Data);
            await using var db = _mySqlDbFactory.GetConnection(state.Identity);
            await db.ExecuteAsync(sql, new SharedTableStateEntity
            {
                Version = state.Version,
                ClaptrapId = state.Identity.Id,
                StateData = stateData,
                UpdatedTime = _clock.UtcNow,
                ClaptrapTypeCode = state.Identity.TypeCode
            });
        }

        public async Task<IState?> GetStateSnapshotAsync()
        {
            await using var db = _mySqlDbFactory.GetConnection(Identity);
            var stateEntity = await db.QuerySingleOrDefaultAsync<SharedTableStateEntity>(_selectSql.Value,
                new {ClaptrapTypeCode = Identity.TypeCode, ClaptrapId = Identity.Id});
            if (stateEntity == null)
            {
                return default;
            }

            var stateData = _stateDataStringSerializer.Deserialize(Identity.TypeCode, stateEntity.StateData);
            var dataState = new DataState(Identity, stateData, stateEntity.Version);
            return dataState;
        }

        public class SharedTableStateEntity
        {
            public string ClaptrapTypeCode { get; set; }
            public string ClaptrapId { get; set; }
            public long Version { get; set; }
            public string StateData { get; set; }
            public DateTime UpdatedTime { get; set; }
        }
    }
}