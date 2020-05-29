using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventStoreMigration : DbUpMysqlMigration
    {
        private readonly IMySqlSharedTableEventStoreOptions _options;

        public MySqlSharedTableEventStoreMigration(
            IMySqlSharedTableEventStoreOptions options,
            IDbFactory dbFactory,
            ILogger<MySqlSharedTableEventStoreMigration> logger) : base(dbFactory,
            logger)
        {
            _options = options;
        }

        protected override string GetDbName()
        {
            return _options.DbName;
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith("-SharedTable.event.sql");
        }

        protected override Dictionary<string, string> GetVariables()
        {
            return new Dictionary<string, string>
            {
                {"SchemaName", _options.SchemaName},
                {"EventTableName", _options.EventTableName},
            };
        }
    }
}