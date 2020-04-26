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
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteEventStore : IEventStore
    {
        private readonly ILogger<SQLiteEventStore> _logger;
        private readonly IEventHandlerRegister _eventHandlerRegister;

        public delegate SQLiteEventStore Factory(IActorIdentity identity);

        private readonly string _connectionString;
        private readonly Lazy<bool> _databaseCreated;
        private readonly Lazy<string> _tableName;

        public SQLiteEventStore(IActorIdentity identity,
            ILogger<SQLiteEventStore> logger,
            IEventHandlerRegister eventHandlerRegister)
        {
            _logger = logger;
            _eventHandlerRegister = eventHandlerRegister;
            Identity = identity;
            _connectionString = ConnectionString();
            _databaseCreated = new Lazy<bool>(() =>
            {
                CreateOrUpdateDatabase();
                return true;
            });
            _tableName = new Lazy<string>(() => $"event_{Identity.TypeCode}_{Identity.Id}");
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
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IEvent>> GetEvents(ulong startVersion, ulong endVersion)
        {
            _ = _databaseCreated.Value;
            await using var db = new SQLiteConnection(_connectionString);
            throw new NotImplementedException();
        }
    }

    public class EventEntity
    {
        public long Version { get; set; }
        public string Uid { get; set; }
        public string EventTypeCode { get; set; }
        public string EventData { get; set; }
        public long CreatedTime { get; set; }
    }
}