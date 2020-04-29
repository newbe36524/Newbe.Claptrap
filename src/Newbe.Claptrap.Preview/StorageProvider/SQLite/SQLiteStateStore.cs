using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Serializer;
using Newbe.Claptrap.Preview.Impl;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public class SQLiteStateStore : IStateLoader, IStateSaver
    {
        private readonly ILogger<SQLiteStateStore> _logger;
        private readonly ISQLiteDbFactory _sqLiteDbFactory;
        private readonly IClock _clock;
        private readonly IStateDataStringSerializer _stateDataStringSerializer;
        private readonly Lazy<string> _selectSql;
        private readonly Lazy<string> _upsertSql;
        private readonly Lazy<bool> _dbCreated;

        public delegate SQLiteStateStore Factory(IClaptrapIdentity identity);

        public IClaptrapIdentity Identity { get; }

        public SQLiteStateStore(
            ILogger<SQLiteStateStore> logger,
            IClaptrapIdentity identity,
            ISQLiteDbFactory sqLiteDbFactory,
            ISQLiteDbManager sqLiteDbManager,
            IClock clock,
            IStateDataStringSerializer stateDataStringSerializer)
        {
            _logger = logger;
            _sqLiteDbFactory = sqLiteDbFactory;
            _clock = clock;
            _stateDataStringSerializer = stateDataStringSerializer;
            Identity = identity;
            _dbCreated = new Lazy<bool>(() =>
            {
                sqLiteDbManager.CreateOrUpdateDatabase(Identity, _sqLiteDbFactory.CreateConnection(Identity));
                return true;
            });
            _selectSql = new Lazy<string>(() =>
                $"SELECT * FROM [{DbHelper.GetStateTableName(Identity)}] WHERE [actortypecode]=@ActorTypeCode AND [id]=@Id LIMIT 1");
            _upsertSql = new Lazy<string>(() =>
                $"INSERT OR REPLACE INTO [{DbHelper.GetStateTableName(Identity)}] ([id],[version],[actortypecode],[statedata],[updatedtime]) VALUES(@Id, @Version, @ActorTypeCode, @StateData, @UpdatedTime)");
        }

        public async Task<IState?> GetStateSnapshotAsync()
        {
            _ = _dbCreated.Value;
            await using var db = _sqLiteDbFactory.CreateConnection(Identity);
            var stateEntity = await db.QuerySingleOrDefaultAsync<StateEntity>(_selectSql.Value,
                new {ActorTypeCode = Identity.TypeCode, Id = Identity.Id});
            if (stateEntity == null)
            {
                return default;
            }

            var stateData = _stateDataStringSerializer.Deserialize(Identity.TypeCode, stateEntity.StateData);
            var dataState = new DataState(Identity, stateData, stateEntity.Version);
            return dataState;
        }

        public async Task SaveAsync(IState state)
        {
            _ = _dbCreated.Value;
            var stateData = _stateDataStringSerializer.Serialize(Identity.TypeCode, state.Data);
            var stateEntity = new StateEntity
            {
                Id = Identity.Id,
                Version = state.Version,
                StateData = stateData,
                UpdatedTime = _clock.UtcNow,
                ActorTypeCode = Identity.TypeCode
            };
            await using var db = _sqLiteDbFactory.CreateConnection(Identity);
            var rowCount = await db.ExecuteAsync(_upsertSql.Value, stateEntity);
            if (rowCount > 0)
            {
                _logger.LogDebug("snapshot upserted");
            }
            else
            {
                // TODO throw exception maybe
                _logger.LogError("snapshot upserted failed");
            }
        }
    }
}