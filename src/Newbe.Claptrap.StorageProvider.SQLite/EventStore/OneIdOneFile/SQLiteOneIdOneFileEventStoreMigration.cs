using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using DbUp.SQLite.Helpers;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileEventStoreMigration :
        IEventLoaderMigration,
        IEventSaverMigration
    {
        private readonly IStorageMigration _migration;

        public SQLiteOneIdOneFileEventStoreMigration(
            IClaptrapIdentity identity,
            ISQLiteOneIdOneFileEventStoreOptions options,
            IDbFactory dbFactory,
            DbUpMigration.Factory factory,
            ILogger<SQLiteOneIdOneFileEventStoreMigration> logger,
            IMasterClaptrapInfo? masterClaptrapInfo = null)
        {
            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-OneIdOneFile.event.sql"),
                new Dictionary<string, string>
                {
                    {"EventTableName", options.EventTableName},
                },
                () =>
                    DeployChanges
                        .To.SQLiteDatabase(new SharedConnection(dbFactory.GetConnection(
                            SQLiteConnectionNameHelper.OneIdOneFileEventStore(masterClaptrapInfo?.Identity ?? identity)))));
            _migration = factory.Invoke(logger, migrationOptions);
        }

        public Task MigrateAsync()
        {
            return _migration.MigrateAsync();
        }
    }
}