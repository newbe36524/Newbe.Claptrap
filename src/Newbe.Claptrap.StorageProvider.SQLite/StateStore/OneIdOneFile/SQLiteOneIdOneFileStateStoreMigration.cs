using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using DbUp.SQLite.Helpers;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileStateStoreMigration :
        IStateLoaderMigration,
        IStateSaverMigration
    {
        private readonly IStorageMigration _migration;

        public SQLiteOneIdOneFileStateStoreMigration(
            IClaptrapIdentity identity,
            IClaptrapDesign claptrapDesign,
            IDbFactory dbFactory,
            DbUpMigration.Factory factory,
            ISQLiteOneIdOneFileStateStoreOptions options,
            ILogger<SQLiteOneIdOneFileStateStoreMigration> logger)
        {
            var migrationOptions = new DbUpMigrationOptions(
                new[] {Assembly.GetExecutingAssembly()},
                fileName => fileName.EndsWith("-OneIdOneFile.state.sql"),
                new Dictionary<string, string>
                {
                    {"StateTableName", options.StateTableName}
                },
                () =>
                    DeployChanges
                        .To.SQLiteDatabase(new SharedConnection(dbFactory.GetConnection(
                            SQLiteConnectionNameHelper.OneIdOneFileStateStore(claptrapDesign, identity)))));
            _migration = factory.Invoke(logger, migrationOptions);
        }

        public Task MigrateAsync()
        {
            return _migration.MigrateAsync();
        }
    }
}