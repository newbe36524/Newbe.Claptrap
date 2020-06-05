using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using DbUp.SQLite.Helpers;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore
{
    public class SQLiteRelationalStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public SQLiteRelationalStateStoreMigration(
            ILogger<SQLiteRelationalStateStoreMigration> logger,
            DbUpMigration.Factory factory,
            IDbFactory dbFactory,
            IClaptrapIdentity identity,
            IStorageMigrationContainer storageMigrationContainer,
            ISQLiteRelationalStateStoreOptions options)
        {
            var locator = options.RelationalStateStoreLocator;
            var stateTableName = locator.GetStateTableName(identity);
            var connectionName = locator.GetConnectionName(identity);

            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-state.sql"),
                new Dictionary<string, string>
                {
                    {"StateTableName", stateTableName},
                },
                () =>
                    DeployChanges
                        .To.SQLiteDatabase(new SharedConnection(dbFactory.GetConnection(connectionName))));

            var migration = factory.Invoke(logger, migrationOptions);

            var migrationKey =
                $"{nameof(SQLiteRelationalStateStoreMigration)}_{connectionName}_{stateTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }


        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}