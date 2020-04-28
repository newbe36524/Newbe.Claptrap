using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteEventStore : IEventStore
    {
        private readonly ILogger<SQLiteEventStore> _logger;
        private readonly IClock _clock;
        private readonly ISQLiteDbFactory _sqLiteDbFactory;
        private readonly ISQLiteDbManager _sqLiteDbManager;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public delegate SQLiteEventStore Factory(IActorIdentity identity);

        private readonly Lazy<bool> _databaseCreated;
        private readonly Lazy<string> _insertSql;
        private readonly Lazy<string> _selectSql;

        public SQLiteEventStore(
            IActorIdentity identity,
            ILogger<SQLiteEventStore> logger,
            IClock clock,
            ISQLiteDbFactory sqLiteDbFactory,
            ISQLiteDbManager sqLiteDbManager,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _logger = logger;
            _clock = clock;
            _sqLiteDbFactory = sqLiteDbFactory;
            _sqLiteDbManager = sqLiteDbManager;
            _eventDataStringSerializer = eventDataStringSerializer;
            Identity = identity;
            _databaseCreated = new Lazy<bool>(() =>
            {
                _sqLiteDbManager.CreateOrUpdateDatabase(Identity, _sqLiteDbFactory.CreateConnection(Identity));
                return true;
            });
            _insertSql = new Lazy<string>(() =>
                $"INSERT OR IGNORE INTO [{DbHelper.GetEventTableName(Identity)}] ([version], [uid], [eventtypecode], [eventdata], [createdtime]) VALUES (@Version, @Uid, @EventTypeCode, @EventData, @CreatedTime)");
            _selectSql = new Lazy<string>(() =>
                $"SELECT * FROM [{DbHelper.GetEventTableName(Identity)}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]");
        }

        public IActorIdentity Identity { get; }

        public async Task<EventSavingResult> SaveEvent(IEvent @event)
        {
            _ = _databaseCreated.Value;
            await using var db = _sqLiteDbFactory.CreateConnection(Identity);
            var eventData = _eventDataStringSerializer.Serialize(Identity.TypeCode, @event.EventTypeCode, @event.Data);
            var eventEntity = new EventEntity
            {
                Version = @event.Version,
                CreatedTime = _clock.UtcNow,
                EventData = eventData,
                EventTypeCode = @event.EventTypeCode,
                Uid = @event.Uid!,
            };
            _logger.LogDebug("start to save event to store @{eventEntity}", eventEntity);
            var rowCount = await db.ExecuteAsync(_insertSql.Value, eventEntity);
            var re = rowCount > 0 ? EventSavingResult.Success : EventSavingResult.AlreadyAdded;
            _logger.LogDebug("event savingResult : {eventSavingResult}", re);
            return re;
        }

        public async Task<IEnumerable<IEvent>> GetEvents(long startVersion, long endVersion)
        {
            _ = _databaseCreated.Value;
            await using var db = _sqLiteDbFactory.CreateConnection(Identity);
            var ps = new {startVersion, endVersion};
            _logger.LogDebug("start to get events that version in range [{startVersion}, {endVersion}).",
                startVersion,
                endVersion);
            var eventEntities = await db.QueryAsync<EventEntity>(_selectSql.Value, ps);

            var re = eventEntities.Select(x =>
            {
                var eventData = _eventDataStringSerializer.Deserialize(Identity.TypeCode, x.EventTypeCode, x.EventData);
                var dataEvent = new DataEvent(Identity, x.EventTypeCode, eventData, x.Uid)
                {
                    Version = x.Version
                };
                return dataEvent;
            }).ToArray();
            _logger.LogDebug("found {count} events that version in range [{startVersion}, {endVersion}).",
                re.Length,
                startVersion,
                endVersion);
            return re;
        }
    }
}