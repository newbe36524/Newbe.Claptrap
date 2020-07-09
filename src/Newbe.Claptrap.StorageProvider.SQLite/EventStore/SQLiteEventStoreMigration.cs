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
    public class SQLiteEventStoreMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly Task _migrationTask;

        public SQLiteEventStoreMigration(
            ILogger<SQLiteEventStoreMigration> logger,
            DbUpMigration.Factory factory,
            ISQLiteDbFactory sqLiteDbFactory,
            IStorageMigrationContainer storageMigrationContainer,
            IMasterOrSelfIdentity masterOrSelfIdentity,
            ISQLiteEventStoreOptions options)
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
                {
                    var dbConnection = sqLiteDbFactory.GetConnection(connectionName);
                    var builder = DeployChanges.To.SQLiteDatabase(new SharedConnection(dbConnection));
                    return (builder, dbConnection);
                });
            var migration = factory.Invoke(logger, migrationOptions);
            var migrationKey =
                $"{nameof(SQLiteEventStoreMigration)}_{connectionName}_{eventTableName}";
            _migrationTask = storageMigrationContainer.CreateTask(migrationKey, migration);
        }

        public Task MigrateAsync()
        {
            return _migrationTask;
        }
    }
}