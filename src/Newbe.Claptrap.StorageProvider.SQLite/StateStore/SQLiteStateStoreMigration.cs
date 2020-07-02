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
    public class SQLiteStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public SQLiteStateStoreMigration(
            ILogger<SQLiteStateStoreMigration> logger,
            DbUpMigration.Factory factory,
            ISQLiteDbFactory sqLiteDbFactory,
            IClaptrapIdentity identity,
            IStorageMigrationContainer storageMigrationContainer,
            ISQLiteStateStoreOptions options)
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
                {
                    var dbConnection = sqLiteDbFactory.GetConnection(connectionName);
                    var builder = DeployChanges.To.SQLiteDatabase(new SharedConnection(dbConnection));
                    return (builder, dbConnection);
                });

            var migration = factory.Invoke(logger, migrationOptions);

            var migrationKey =
                $"{nameof(SQLiteStateStoreMigration)}_{connectionName}_{stateTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }


        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}