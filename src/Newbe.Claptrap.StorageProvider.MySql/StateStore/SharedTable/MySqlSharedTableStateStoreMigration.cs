using System.Collections.Generic;
using System.Threading.Tasks;
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
            DbUpMysqlMigration.Factory factory,
            IStorageMigrationContainer storageMigrationContainer,
            IMySqlSharedTableStateStoreOptions options)
        {
            var migration = factory.Invoke(logger, new DbUpMysqlMigrationOptions
            {
                Variables = new Dictionary<string, string>
                {
                    {"SchemaName", options.SchemaName},
                    {"StateTableName", options.StateTableName},
                },
                DbName = options.DbName,
                SqlSelector = fileName => fileName.EndsWith("-SharedTable.state.sql"),
            });
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