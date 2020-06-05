using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.Extensions;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore
{
    public class PostgreSQLStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public PostgreSQLStateStoreMigration(
            ILogger<PostgreSQLStateStoreMigration> logger,
            DbUpMigration.Factory factory,
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IPostgreSQLStateStoreOptions options)
        {
            var locator = options.RelationalStateStoreLocator;
            var (connectionName, schemaName, stateTableName) = locator.GetNames(identity);

            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-state.sql"),
                new Dictionary<string, string>
                {
                    {"SchemaName", schemaName},
                    {"StateTableName", stateTableName},
                },
                () =>
                    DeployChanges
                        .To.PostgresqlDatabase(dbFactory.GetConnectionString(connectionName)));

            var migration = factory.Invoke(logger, migrationOptions);

            var migrationKey =
                $"{nameof(PostgreSQLStateStoreMigration)}_{connectionName}_{schemaName}_{stateTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }


        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}