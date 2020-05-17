using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlDbManager : IMySqlDbManager
    {
        private readonly IMySqlDbFactory _mySqlDbFactory;
        private readonly ILogger<MySqlDbManager> _logger;

        public MySqlDbManager(
            IMySqlDbFactory mySqlDbFactory,
            ILogger<MySqlDbManager> logger)
        {
            _mySqlDbFactory = mySqlDbFactory;
            _logger = logger;
        }

        public void CreateOrUpdateDatabase(IClaptrapIdentity identity,
            Func<string, bool> sqlSelector,
            Dictionary<string, string> variables)
        {
            var connectionString = _mySqlDbFactory.GetConnectionString(identity);
            MigrationDb(connectionString, variables);

            void MigrationDb(string conn, IDictionary<string, string> data)
            {
                var dbMigration =
                    DeployChanges.To
                        .MySqlDatabase(conn)
                        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), sqlSelector)
                        .LogToAutodetectedLog()
                        .LogToConsole()
                        .JournalTo(new NullJournal())
                        .WithVariablesEnabled()
                        .WithVariables(data)
                        .Build();

                var result = dbMigration.PerformUpgrade();

                if (!result.Successful)
                {
                    throw new Exception(
                        $"db migration failed, {identity}",
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