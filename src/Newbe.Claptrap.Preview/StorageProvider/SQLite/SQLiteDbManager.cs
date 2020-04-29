using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DbUp;
using DbUp.Engine;
using DbUp.SQLite.Helpers;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public class SQLiteDbManager : ISQLiteDbManager
    {
        private readonly ILogger<SQLiteDbManager> _logger;

        public SQLiteDbManager(
            ILogger<SQLiteDbManager> logger)
        {
            _logger = logger;
        }

        public void CreateOrUpdateDatabase(IClaptrapIdentity claptrapIdentity, IDbConnection dbConnection)
        {
            var dir = DbHelper.GetDatabaseDirectory();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                _logger.LogInformation("directory {databaseDir} for SQLite event store not found, created", dir);
            }

            var dbMigration =
                DeployChanges.To
                    .SQLiteDatabase(new SharedConnection(dbConnection))
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToAutodetectedLog()
                    .LogToConsole()
                    .WithVariablesEnabled()
                    .WithVariable("ActorTypeCode", claptrapIdentity.TypeCode)
                    .WithVariable("ActorId", claptrapIdentity.Id)
                    .WithVariable("EventTableName", DbHelper.GetEventTableName(claptrapIdentity))
                    .WithVariable("StateTableName", DbHelper.GetStateTableName(claptrapIdentity))
                    .Build();

            var result = dbMigration.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception(
                    $"event store create failed for {claptrapIdentity.TypeCode} {claptrapIdentity.Id}",
                    result.Error);
            }

            if (result.Scripts.Any())
            {
                var dbFilename = DbHelper.GetDbFilename(claptrapIdentity);
                _logger.LogInformation("db migration for {filename} is success.", dbFilename);
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

        public void DeleteIfFound(IClaptrapIdentity claptrapIdentity)
        {
            var filename = DbHelper.GetDbFilename(claptrapIdentity);
            if (File.Exists(filename))
            {
                _logger.LogInformation("db file found, start to delete it. path: {path}", filename);
                File.Delete(filename);
            }

            _logger.LogInformation("there is no db file, do nothing. path: {path}", filename);
        }
    }
}