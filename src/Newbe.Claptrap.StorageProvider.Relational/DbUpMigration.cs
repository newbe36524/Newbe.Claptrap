using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbUp.Engine;
using DbUp.Helpers;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class DbUpMigration :
        IStorageMigration
    {
        public delegate DbUpMigration Factory(ILogger logger,
            DbUpMigrationOptions options);

        private readonly ILogger _logger;
        private readonly DbUpMigrationOptions _options;

        public DbUpMigration(
            ILogger logger,
            DbUpMigrationOptions options)
        {
            _logger = logger;
            _options = options;
        }

        public Task MigrateAsync()
        {
            CreateOrUpdateDatabase();
            return Task.CompletedTask;
        }

        private void CreateOrUpdateDatabase()
        {
            MigrationCore();

            void MigrationCore()
            {
                var (builder, dbConnection) = _options.UpgradeEngineBuilderFactory();
                foreach (var scriptAssembly in _options.ScriptAssemblies)
                {
                    builder
                        .WithScriptsEmbeddedInAssembly(scriptAssembly, _options.ScriptSelector);
                }

                builder
                    .LogToAutodetectedLog()
                    .WithVariablesEnabled()
                    .JournalTo(new NullJournal())
                    .WithVariables(_options.Variables);

                var dbMigration = builder.Build();

                var result = dbMigration.PerformUpgrade();
                dbConnection?.Dispose();

                if (!result.Successful)
                {
                    throw new Exception(
                        "db migration failed",
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