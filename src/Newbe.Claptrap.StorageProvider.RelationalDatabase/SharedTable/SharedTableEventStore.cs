using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public class SharedTableEventStore : IEventLoader, IEventSaver
    {
        private readonly IClock _clock;
        private readonly ILogger<SharedTableEventStore> _logger;
        private readonly IShareTableEventStoreProvider _shareTableEventStoreProvider;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;
        private readonly IDbFactory _dbFactory;

        public delegate SharedTableEventStore Factory(IClaptrapIdentity identity);

        public SharedTableEventStore(IClaptrapIdentity identity,
            IClock clock,
            ILogger<SharedTableEventStore> logger,
            IShareTableEventStoreProvider shareTableEventStoreProvider,
            IEventDataStringSerializer eventDataStringSerializer,
            IDbFactory dbFactory)
        {
            _clock = clock;
            _logger = logger;
            _shareTableEventStoreProvider = shareTableEventStoreProvider;
            _eventDataStringSerializer = eventDataStringSerializer;
            _dbFactory = dbFactory;
            Identity = identity;
        }

        // private bool InitDb()
        // {
        //     var def = _factory.Invoke();
        //     _mySqlDbManager.CreateOrUpdateDatabase(Identity, EventSqlSelector, new CreateEventTableVars
        //     {
        //         SchemaName = def.SchemaName,
        //         EventTableName = def.EventTableName,
        //     }.GetDictionary());
        //     return true;
        //
        //     static bool EventSqlSelector(string file)
        //         => file.EndsWith("event-mysql_default_table.sql");
        // }


        public IClaptrapIdentity Identity { get; }

        public async Task SaveEventAsync(IEvent @event)
        {
            try
            {
                await SaveEventAsyncCore();
            }
            catch (Exception e)
            {
                throw new EventSavingException(e, @event);
            }

            async Task SaveEventAsyncCore()
            {
                var insertOneSql = _shareTableEventStoreProvider.CreateInsertOneSql(Identity);
                var identity = @event.ClaptrapIdentity;
                var eventData = _eventDataStringSerializer.Serialize(
                    identity.TypeCode,
                    @event.EventTypeCode,
                    @event.Data);
                using var db = _dbFactory.GetConnection(Identity);
                await db.ExecuteAsync(insertOneSql, new DefaultTableEventEntity
                {
                    Version = @event.Version,
                    CreatedTime = _clock.UtcNow,
                    EventData = eventData,
                    ClaptrapId = identity.Id,
                    ClaptrapTypeCode = identity.TypeCode,
                    EventTypeCode = @event.EventTypeCode
                });
            }
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            var ps = new {startVersion, endVersion};
            using var db = _dbFactory.GetConnection(Identity);
            var selectSql = _shareTableEventStoreProvider.CreateSelectSql(Identity);
            var eventEntities = await db.QueryAsync<DefaultTableEventEntity>(selectSql, ps);

            var re = eventEntities.Select(x =>
            {
                var eventData = _eventDataStringSerializer.Deserialize(Identity.TypeCode, x.EventTypeCode, x.EventData);
                var dataEvent = new DataEvent(Identity, x.EventTypeCode, eventData)
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

        public class DefaultTableEventEntity
        {
            public string ClaptrapTypeCode { get; set; }
            public string ClaptrapId { get; set; }
            public long Version { get; set; }
            public string EventTypeCode { get; set; }
            public string EventData { get; set; }
            public DateTime CreatedTime { get; set; }
        }
    }
}