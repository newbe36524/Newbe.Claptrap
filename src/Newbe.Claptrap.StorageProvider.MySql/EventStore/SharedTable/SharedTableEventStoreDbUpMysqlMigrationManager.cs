using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class SharedTableEventStoreDbUpMysqlMigrationManager : DbUpMysqlMigrationManager
    {
        public delegate SharedTableEventStoreDbUpMysqlMigrationManager Factory();
        private readonly MySqlSharedTableEventStoreConfig _mySqlSharedTableEventStoreConfig;

        public SharedTableEventStoreDbUpMysqlMigrationManager(
            MySqlSharedTableEventStoreConfig mySqlSharedTableEventStoreConfig,
            IDbFactory dbFactory,
            ILogger logger) : base(dbFactory,
            logger)
        {
            _mySqlSharedTableEventStoreConfig = mySqlSharedTableEventStoreConfig;
        }

        protected override string GetDbName()
        {
            return _mySqlSharedTableEventStoreConfig.SharedTableEventStoreDbName;
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith("event-mysql_shared_table.sql");
        }

        protected override Dictionary<string, string> GetVariables()
        {
            return new Dictionary<string, string>
            {
                {"EventTableName", _mySqlSharedTableEventStoreConfig.EventTableName},
                {"Schema", _mySqlSharedTableEventStoreConfig.SchemaName},
            };
        }
    }
}