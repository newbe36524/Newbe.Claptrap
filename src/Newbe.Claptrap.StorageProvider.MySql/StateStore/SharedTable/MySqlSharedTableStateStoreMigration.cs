using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.MySql.StateStore.SharedTable
{
    public class MySqlSharedTableStateStoreMigration : DbUpMysqlMigration
    {
        private readonly IMySqlSharedTableStateStoreOptions _options;

        public MySqlSharedTableStateStoreMigration(
            IDbFactory dbFactory,
            IMySqlSharedTableStateStoreOptions options,
            ILogger<MySqlSharedTableStateStoreMigration> logger) : base(dbFactory, logger)
        {
            _options = options;
        }

        protected override string GetDbName()
        {
            return _options.DbName;
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith("-SharedTable.state.sql");
        }

        protected override Dictionary<string, string> GetVariables()
        {
            return new Dictionary<string, string>
            {
                {"SchemaName", _options.SchemaName},
                {"StateTableName", _options.StateTableName},
            };
        }
    }
}