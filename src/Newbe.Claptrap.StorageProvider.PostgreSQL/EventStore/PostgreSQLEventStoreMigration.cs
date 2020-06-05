using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Extensions;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore
{
    public class PostgreSQLEventStoreMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly Task _migrationTask;

        public PostgreSQLEventStoreMigration(
            ILogger<PostgreSQLEventStoreMigration> logger,
            DbUpMigration.Factory factory,
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IPostgreSQLEventStoreOptions options)
        {
            var (connectionName, schemaName, eventTableName) =
                options.RelationalEventStoreLocator.GetNames(identity);
            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-event.sql"),
                new Dictionary<string, string>
                {
                    {"SchemaName", schemaName},
                    {"EventTableName", eventTableName},
                },
                () =>
                    DeployChanges
                        .To.PostgresqlDatabase(dbFactory.GetConnectionString(connectionName)));

            var migration = factory.Invoke(logger, migrationOptions);
            var migrationKey =
                $"{nameof(PostgreSQLEventStoreMigration)}_{connectionName}_{schemaName}_{eventTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }

        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}