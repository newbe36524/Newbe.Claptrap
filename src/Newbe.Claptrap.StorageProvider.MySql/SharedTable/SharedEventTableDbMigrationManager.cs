using System.Collections.Generic;
using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.RelationalDatabase;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.SharedTable
{
    public class SharedEventTableDbMigrationManager : IDbMigrationManager
    {
        public delegate SharedEventTableDbMigrationManager Factory(IClaptrapIdentity identity,
            SharedTableMigrationVar sharedTableMigrationVar);

        private readonly IClaptrapIdentity _identity;
        private readonly SharedTableMigrationVar _sharedTableMigrationVar;
        private readonly IMySqlDbManager _mySqlDbManager;

        public SharedEventTableDbMigrationManager(
            IClaptrapIdentity identity,
            SharedTableMigrationVar sharedTableMigrationVar,
            IMySqlDbManager mySqlDbManager)
        {
            _identity = identity;
            _sharedTableMigrationVar = sharedTableMigrationVar;
            _mySqlDbManager = mySqlDbManager;
        }

        public Task Migrate()
        {
            _mySqlDbManager.CreateOrUpdateDatabase(_identity, EventSqlSelector, new Dictionary<string, string>
            {
                {nameof(_sharedTableMigrationVar.SchemaName), _sharedTableMigrationVar.SchemaName},
                {nameof(_sharedTableMigrationVar.EventTableName), _sharedTableMigrationVar.EventTableName},
            });

            static bool EventSqlSelector(string file)
                => file.EndsWith("event-mysql_default_table.sql");

            return Task.CompletedTask;
        }
    }
}