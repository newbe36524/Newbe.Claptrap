using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.StateStore
{
    public class MySqlStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public MySqlStateStoreMigration(
            ILogger<MySqlStateStoreMigration> logger,
            DbUpMigration.Factory factory,
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IRelationalStateStoreLocatorOptions options)
        {
            var locator = options.RelationalStateStoreLocator;
            var connectionName = locator.GetConnectionName(identity);
            var schemaName = locator.GetSchemaName(identity);
            var stateTableName = locator.GetStateTableName(identity);
            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-Relational.state.sql"),
                new Dictionary<string, string>
                {
                    {"SchemaName", schemaName},
                    {"StateTableName", stateTableName},
                },
                () =>
                    DeployChanges
                        .To.MySqlDatabase(dbFactory.GetConnectionString(connectionName)));

            var migration = factory.Invoke(logger, migrationOptions);
            var migrationKey =
                $"{nameof(MySqlStateStoreMigration)}_{connectionName}_{schemaName}_{stateTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }


        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}