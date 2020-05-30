using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore.SharedTable
{
    public class PostgreSQLSharedTableStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public PostgreSQLSharedTableStateStoreMigration(
            ILogger<PostgreSQLSharedTableStateStoreMigration> logger,
            DbUpMigration.Factory factory,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IPostgreSQLSharedTableStateStoreOptions options)
        {
            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-SharedTable.state.sql"),
                new Dictionary<string, string>
                {
                    {"SchemaName", options.SchemaName},
                    {"StateTableName", options.StateTableName},
                },
                () =>
                    DeployChanges
                        .To.PostgresqlDatabase(dbFactory.GetConnectionString(options.DbName)),
                true);

            var migration = factory.Invoke(logger, migrationOptions);

            var migrationKey =
                $"{nameof(PostgreSQLSharedTableStateStoreMigration)}_{options.DbName}_{options.SchemaName}_{options.StateTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }


        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}