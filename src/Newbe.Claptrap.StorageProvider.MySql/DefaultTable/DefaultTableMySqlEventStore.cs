using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.MySql.DefaultTable
{
    public class DefaultTableMySqlEventStore : IEventLoader, IEventSaver
    {
        private readonly SharedTableEventTableDef.Factory _factory;
        private readonly IClock _clock;
        private readonly ILogger<DefaultTableMySqlEventStore> _logger;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;
        private readonly IMySqlDbManager _mySqlDbManager;
        private readonly IMySqlDbFactory _mySqlDbFactory;

        public delegate DefaultTableMySqlEventStore Factory(IClaptrapIdentity identity);

        private readonly Lazy<string> _insertSql;
        private readonly Lazy<string> _selectSql;

        public DefaultTableMySqlEventStore(IClaptrapIdentity identity,
            SharedTableEventTableDef.Factory factory,
            IClock clock,
            ILogger<DefaultTableMySqlEventStore> logger,
            IEventDataStringSerializer eventDataStringSerializer,
            IMySqlDbManager mySqlDbManager,
            IMySqlDbFactory mySqlDbFactory)
        {
            _factory = factory;
            _clock = clock;
            _logger = logger;
            _eventDataStringSerializer = eventDataStringSerializer;
            _mySqlDbManager = mySqlDbManager;
            _mySqlDbFactory = mySqlDbFactory;
            Identity = identity;
            _insertSql = new Lazy<string>(() =>
            {
                var def = _factory.Invoke();
                return
                    $"INSERT INTO [{def.SchemaName}].[{def.EventTableName}] ([claptrap_type_code], [claptrap_id], [version], [event_type_code], [event_data], [created_time]) VALUES (@ClaptrapTypeCode, @ClaptrapId, @Version, @EventTypeCode, @EventData, @CreatedTime)";
            });

            _selectSql = new Lazy<string>(() =>
            {
                var def = _factory.Invoke();
                return
                    $"SELECT * FROM [{def.SchemaName}].[{def.EventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]";
            });
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
                var insertSql = _insertSql.Value;
                var identity = @event.ClaptrapIdentity;
                var eventData = _eventDataStringSerializer.Serialize(
                    identity.TypeCode,
                    @event.EventTypeCode,
                    @event.Data);
                await using var db = _mySqlDbFactory.GetConnection(Identity);
                await db.ExecuteAsync(insertSql, new DefaultTableEventEntity
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
            await using var db = _mySqlDbFactory.GetConnection(Identity);
            var eventEntities = await db.QueryAsync<DefaultTableEventEntity>(_selectSql.Value, ps);

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