using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public abstract class DbUpMysqlMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly IDbFactory _dbFactory;
        private readonly ILogger _logger;

        protected DbUpMysqlMigration(
            IDbFactory dbFactory,
            ILogger logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public Task MigrateAsync()
        {
            var dbName = GetDbName();
            var dictionary = GetVariables();
            CreateOrUpdateDatabase(dbName, SqlSelector, dictionary);
            return Task.CompletedTask;
        }

        protected abstract string GetDbName();
        protected abstract bool SqlSelector(string fileName);
        protected abstract Dictionary<string, string> GetVariables();

        private void CreateOrUpdateDatabase(
            string dbName,
            Func<string, bool> sqlSelector,
            IDictionary<string, string> variables)
        {
            var connectionString = _dbFactory.GetConnectionString(dbName);
            MigrationDb(connectionString, variables);

            void MigrationDb(string conn, IDictionary<string, string> data)
            {
                var dbMigration =
                    DeployChanges.To
                        .MySqlDatabase(conn)
                        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), sqlSelector)
                        .LogToAutodetectedLog()
                        .JournalTo(new NullJournal())
                        .WithVariablesEnabled()
                        .WithVariables(data)
                        .Build();

                var result = dbMigration.PerformUpgrade();

                if (!result.Successful)
                {
                    throw new Exception(
                        $"db migration failed",
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
            foreach (var script in result.Scripts)
            {
                sb.AppendLine(script.Name);
                sb.AppendLine(script.Contents);
            }

            sb.AppendLine("##octopus[stdout-default]");
            return sb.ToString();
        }
    }
}