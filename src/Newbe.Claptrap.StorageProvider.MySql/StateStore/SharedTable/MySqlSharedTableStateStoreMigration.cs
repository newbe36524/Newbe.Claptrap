using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.StateStore.SharedTable
{
    public class MySqlSharedTableStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly Task _migrationTask;

        public MySqlSharedTableStateStoreMigration(
            ILogger<MySqlSharedTableStateStoreMigration> logger,
            DbUpMigration.Factory factory,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IMySqlSharedTableStateStoreOptions options)
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
                        .To.MySqlDatabase(dbFactory.GetConnectionString(options.DbName)),
                true);

            var migration = factory.Invoke(logger, migrationOptions);
            var migrationKey =
                $"{nameof(MySqlSharedTableStateStoreMigration)}_{options.DbName}_{options.SchemaName}_{options.StateTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }


        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}