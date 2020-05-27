using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DbUp;
using DbUp.Engine;
using DbUp.SQLite.Helpers;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteDbManager : ISQLiteDbManager
    {
        private readonly DbFilePath.Factory _factory;
        private readonly ISQLiteDbFactory _sqLiteDbFactory;
        private readonly ILogger<SQLiteDbManager> _logger;

        public SQLiteDbManager(
            DbFilePath.Factory factory,
            ISQLiteDbFactory sqLiteDbFactory,
            ILogger<SQLiteDbManager> logger)
        {
            _factory = factory;
            _sqLiteDbFactory = sqLiteDbFactory;
            _logger = logger;
        }

        public void CreateOrUpdateDatabase(IClaptrapIdentity claptrapIdentity)
        {
            var dbFilePath = _factory.Invoke(claptrapIdentity);
            dbFilePath.EnsureDirectoryCreated();

            var ps = new Dictionary<string, string>
            {
                {"ActorTypeCode", claptrapIdentity.TypeCode},
                {"ActorId", claptrapIdentity.Id},
                {"StateTableName", DbHelper.GetStateTableName(claptrapIdentity)},
            };
            MigrationDb(_sqLiteDbFactory.GetStateDbConnection(claptrapIdentity), StateSqlSelector, ps);

            static bool StateSqlSelector(string file)
                =>  file.EndsWith(".state.sql");

            void MigrationDb(IDbConnection db, Func<string, bool> sqlSelector, IDictionary<string, string> data)
            {
                var dbMigration =
                    DeployChanges.To
                        .SQLiteDatabase(new SharedConnection(db))
                        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), sqlSelector)
                        .LogToAutodetectedLog()
                        .LogToConsole()
                        .WithVariablesEnabled()
                        .WithVariables(data)
                        .Build();

                var result = dbMigration.PerformUpgrade();

                if (!result.Successful)
                {
                    throw new Exception(
                        $"db migration failed, {claptrapIdentity}",
                        result.Error);
                }

                if (result.Scripts.Any())
                {
                    _logger.LogInformation("db migration is success.");
                }
                else
                {
                    _logger.LogDebug("db schema is latest, do nothing to migration");
                }

                _logger.LogDebug("db migration log:{log}", WriteExecutedScriptsToOctopusTaskSummary(result));
            }
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
    }
}