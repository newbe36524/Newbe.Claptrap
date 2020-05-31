using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable
{
    public class PostgreSQLSharedTableEventStoreMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly Task _migrationTask;

        public PostgreSQLSharedTableEventStoreMigration(
            ILogger<PostgreSQLSharedTableEventStoreMigration> logger,
            DbUpMigration.Factory factory,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IPostgreSQLSharedTableEventStoreOptions options)
        {
            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-SharedTable.event.sql"),
                new Dictionary<string, string>
                {
                    {"SchemaName", options.SchemaName},
                    {"EventTableName", options.EventTableName},
                },
                () =>
                    DeployChanges
                        .To.PostgresqlDatabase(dbFactory.GetConnectionString(options.ConnectionName)),
                true);

            var migration = factory.Invoke(logger, migrationOptions);
            var migrationKey =
                $"{nameof(PostgreSQLSharedTableEventStoreMigration)}_{options.ConnectionName}_{options.SchemaName}_{options.EventTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }

        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}