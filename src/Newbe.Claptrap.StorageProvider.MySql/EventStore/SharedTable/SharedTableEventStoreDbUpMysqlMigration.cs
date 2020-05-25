using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.MySql.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class SharedTableEventStoreDbUpMysqlMigration : DbUpMysqlMigration
    {
        public delegate SharedTableEventStoreDbUpMysqlMigration Factory(
            IMySqlSharedTableEventStoreOptions options);

        private readonly IMySqlSharedTableEventStoreOptions _options;

        public SharedTableEventStoreDbUpMysqlMigration(
            IMySqlSharedTableEventStoreOptions options,
            IDbFactory dbFactory,
            ILogger logger) : base(dbFactory,
            logger)
        {
            _options = options;
        }

        protected override string GetDbName()
        {
            return _options.SharedTableEventStoreDbName;
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith("event-mysql_shared_table.sql");
        }

        protected override Dictionary<string, string> GetVariables()
        {
            return new Dictionary<string, string>
            {
                {"EventTableName", _options.EventTableName},
                {"Schema", _options.SchemaName},
            };
        }
    }
}