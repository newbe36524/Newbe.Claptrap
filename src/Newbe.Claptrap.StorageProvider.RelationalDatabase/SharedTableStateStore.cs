using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public class SharedTableStateStore : IStateLoader, IStateSaver
    {
        private readonly IClock _clock;
        private readonly ILogger<SharedTableStateStore> _logger;
        private readonly IStateDataStringSerializer _stateDataStringSerializer;
        private readonly IShareTableStateStoreProvider _shareTableStateStoreProvider;
        private readonly IDbFactory _dbFactory;

        public delegate SharedTableStateStore Factory(IClaptrapIdentity identity);

        public SharedTableStateStore(IClaptrapIdentity identity,
            IClock clock,
            IStateDataStringSerializer stateDataStringSerializer,
            ILogger<SharedTableStateStore> logger,
            IShareTableStateStoreProvider shareTableStateStoreProvider,
            IDbFactory dbFactory)
        {
            _clock = clock;
            _stateDataStringSerializer = stateDataStringSerializer;
            _logger = logger;
            _shareTableStateStoreProvider = shareTableStateStoreProvider;
            _dbFactory = dbFactory;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public async Task SaveAsync(IState state)
        {
            var sql = _shareTableStateStoreProvider.CreateUpsertSql(Identity);
            var stateData = _stateDataStringSerializer.Serialize(state.Identity.TypeCode, state.Data);
            // todo NULL
            using var db = _dbFactory.GetConnection("null");
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
            // todo NULL
            using var db = _dbFactory.GetConnection("null");
            var sql = _shareTableStateStoreProvider.CreateSelectSql(Identity);
            var stateEntity = await db.QuerySingleOrDefaultAsync<SharedTableStateEntity>(sql,
                new
                {
                    ClaptrapTypeCode = Identity.TypeCode,
                    ClaptrapId = Identity.Id
                });
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