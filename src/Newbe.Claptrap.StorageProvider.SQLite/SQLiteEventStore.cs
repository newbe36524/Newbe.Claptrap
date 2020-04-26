using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteEventStore : IEventStore
    {
        private readonly ILogger<SQLiteEventStore> _logger;
        private readonly IClock _clock;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public delegate SQLiteEventStore Factory(IActorIdentity identity);

        private readonly string _connectionString;
        private readonly Lazy<bool> _databaseCreated;
        private readonly Lazy<string> _tableName;
        private readonly Lazy<string> _insertSql;
        private readonly Lazy<string> _selectSql;

        public SQLiteEventStore(
            IActorIdentity identity,
            ILogger<SQLiteEventStore> logger,
            IClock clock,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _logger = logger;
            _clock = clock;
            _eventDataStringSerializer = eventDataStringSerializer;
            Identity = identity;
            _connectionString = ConnectionString();
            _databaseCreated = new Lazy<bool>(() =>
            {
                CreateOrUpdateDatabase();
                return true;
            });
            _tableName = new Lazy<string>(() => $"event_{Identity.TypeCode}_{Identity.Id}");
            _insertSql = new Lazy<string>(() =>
                $"INSERT OR IGNORE INTO '{_tableName.Value}' ([version], [uid], [eventtypecode], [eventdata], [createdtime]) VALUES (@Version, @Uid, @EventTypeCode, @EventData, @CreatedTime)");
            _selectSql = new Lazy<string>(() =>
                $"SELECT * FROM '{_tableName.Value}' WHERE version >= @startVersion AND version < @endVersion");
        }

        private static string GetDatabaseDirectory()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "claptrapStorage");
            return dir;
        }

        private string GetDbFilename()
        {
            return Path.Combine(GetDatabaseDirectory(), $"{Identity.TypeCode}_{Identity.Id}.db");
        }

        private string ConnectionString()
        {
            var fileName = GetDbFilename();
            var re = $"Data Source={fileName};";
            return re;
        }

        public void DeleteDatabaseIfFound()
        {
            var filename = GetDbFilename();
            if (File.Exists(filename))
            {
                _logger.LogInformation("db file found, start to delete it. path: {path}", filename);
                File.Delete(filename);
            }

            _logger.LogInformation("there is no db file, do nothing. path: {path}", filename);
        }

        public void CreateOrUpdateDatabase()
        {
            var dir = GetDatabaseDirectory();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                _logger.LogInformation("directory {databaseDir} for SQLite event store not found, created", dir);
            }

            var dbMigration =
                DeployChanges.To
                    .SQLiteDatabase(_connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToAutodetectedLog()
                    .LogToConsole()
                    .WithVariablesEnabled()
                    .WithVariable("ActorTypeCode", Identity.TypeCode)
                    .WithVariable("ActorId", Identity.Id)
                    .WithVariable("EventTableName", _tableName.Value)
                    .Build();

            var result = dbMigration.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception($"event store create failed for {Identity.TypeCode} {Identity.Id}", result.Error);
            }

            if (result.Scripts.Any())
            {
                _logger.LogInformation("db migration for {filename} is success.", GetDbFilename());
            }
            else
            {
                _logger.LogDebug("db schema is latest, do nothing to migration");
            }

            _logger.LogDebug("db migration log:{log}", WriteExecutedScriptsToOctopusTaskSummary(result));
        }

        private static string WriteExecutedScriptsToOctopusTaskSummary(DatabaseUpgradeResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine("##octopus[stdout-highlight]");
            sb.AppendLine($"Ran {result.Scripts.Count()} script{(result.Scripts.Count() == 1 ? "" : "s")}");
            foreach (SqlScript script in result.Scripts)
            {
                sb.AppendLine(script.Name);
                sb.AppendLine(script.Contents);
            }

            sb.AppendLine("##octopus[stdout-default]");
            return sb.ToString();
        }

        public IActorIdentity Identity { get; }

        public async Task<EventSavingResult> SaveEvent(IEvent @event)
        {
            _ = _databaseCreated.Value;
            await using var db = new SQLiteConnection(_connectionString);
            var eventData = _eventDataStringSerializer.Serialize(Identity.TypeCode, @event.EventTypeCode, @event.Data);
            var eventEntity = new EventEntity
            {
                Version = (long) @event.Version,
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
            await using var db = new SQLiteConnection(_connectionString);
            var eventEntities = await db.QueryAsync<EventEntity>(_selectSql.Value,
                new {startVersion, endVersion});

            var re = eventEntities.Select(x =>
            {
                var eventData = _eventDataStringSerializer.Deserialize(Identity.TypeCode, x.EventTypeCode, x.EventData);
                var dataEvent = new DataEvent(Identity, x.EventTypeCode, eventData, x.Uid)
                {
                    Version = x.Version
                };
                return dataEvent;
            }).ToArray();
            return re;
        }
    }

    public class EventEntity
    {
        public long Version { get; set; }
        public string Uid { get; set; }
        public string EventTypeCode { get; set; }
        public string EventData { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}