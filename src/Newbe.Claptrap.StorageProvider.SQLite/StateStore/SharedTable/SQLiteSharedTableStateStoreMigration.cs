using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using DbUp.SQLite.Helpers;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.SQLite.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.SharedTable
{
    public class SQLiteSharedTableStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public SQLiteSharedTableStateStoreMigration(
            ILogger<SQLiteSharedTableStateStoreMigration> logger,
            DbUpMigration.Factory factory,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            ISQLiteSharedTableStateStoreOptions options)
        {
            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-SharedTable.state.sql"),
                new Dictionary<string, string>
                {
                    {"StateTableName", options.StateTableName},
                },
                () =>
                    DeployChanges
                        .To.SQLiteDatabase(new SharedConnection(dbFactory.GetConnection(options.ConnectionName))),
                true);

            var migration = factory.Invoke(logger, migrationOptions);

            var migrationKey =
                $"{nameof(SQLiteSharedTableStateStoreMigration)}_{options.ConnectionName}_{options.StateTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }


        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}