using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore
{
    public class MySqlEventStoreMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly Task _migrationTask;

        public MySqlEventStoreMigration(
            ILogger<MySqlEventStoreMigration> logger,
            DbUpMigration.Factory factory,
            IDbFactory dbFactory,
            IClaptrapIdentity identity,
            IStorageMigrationContainer storageMigrationContainer,
            IRelationalEventStoreLocatorOptions options)
        {
            var locator = options.RelationalEventStoreLocator;
            var connectionName = locator.GetConnectionName(identity);
            var schemaName = locator.GetSchemaName(identity);
            var eventTableName = locator.GetEventTableName(identity);

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
                        .To.MySqlDatabase(dbFactory.GetConnectionString(connectionName)));
            var migration = factory.Invoke(logger, migrationOptions);
            var migrationKey =
                $"{nameof(MySqlEventStoreMigration)}_{connectionName}_{schemaName}_{eventTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }

        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}