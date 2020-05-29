using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventStoreMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly Task _migrationTask;

        public MySqlSharedTableEventStoreMigration(
            ILogger<MySqlSharedTableEventStoreMigration> logger,
            DbUpMysqlMigration.Factory factory,
            IStorageMigrationContainer storageMigrationContainer,
            IMySqlSharedTableEventStoreOptions options)
        {
            var migration = factory.Invoke(logger, new DbUpMysqlMigrationOptions
            {
                Variables = new Dictionary<string, string>
                {
                    {"SchemaName", options.SchemaName},
                    {"EventTableName", options.EventTableName},
                },
                DbName = options.DbName,
                SqlSelector = fileName => fileName.EndsWith("-SharedTable.event.sql"),
            });
            var migrationKey =
                $"{nameof(MySqlSharedTableEventStoreMigration)}_{options.DbName}_{options.SchemaName}_{options.EventTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }

        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}