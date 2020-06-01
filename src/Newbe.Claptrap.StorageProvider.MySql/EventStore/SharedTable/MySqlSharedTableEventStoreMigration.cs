using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
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
            DbUpMigration.Factory factory,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IMySqlSharedTableEventStoreOptions options)
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
                        .To.MySqlDatabase(dbFactory.GetConnectionString(options.ConnectionName)));
            var migration = factory.Invoke(logger, migrationOptions);
            var migrationKey =
                $"{nameof(MySqlSharedTableEventStoreMigration)}_{options.ConnectionName}_{options.SchemaName}_{options.EventTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }

        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}