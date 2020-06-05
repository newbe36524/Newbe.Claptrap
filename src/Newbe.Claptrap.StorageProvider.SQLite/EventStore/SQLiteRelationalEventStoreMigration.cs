using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using DbUp.SQLite.Helpers;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore
{
    public class SQLiteRelationalEventStoreMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly Task _migrationTask;

        public SQLiteRelationalEventStoreMigration(
            ILogger<SQLiteRelationalEventStoreMigration> logger,
            DbUpMigration.Factory factory,
            IDbFactory dbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IMasterOrSelfIdentity masterOrSelfIdentity,
            ISQLiteRelationalEventStoreOptions options)
        {
            var identity = masterOrSelfIdentity.Identity;
            var storeLocator = options.RelationalEventStoreLocator;
            var connectionName = storeLocator.GetConnectionName(identity);
            var eventTableName = storeLocator.GetEventTableName(identity);
            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-event.sql"),
                new Dictionary<string, string>
                {
                    {"EventTableName", eventTableName},
                },
                () =>
                    DeployChanges
                        .To.SQLiteDatabase(new SharedConnection(dbFactory.GetConnection(connectionName))));
            var migration = factory.Invoke(logger, migrationOptions);
            var migrationKey =
                $"{nameof(SQLiteRelationalEventStoreMigration)}_{connectionName}_{eventTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }

        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}