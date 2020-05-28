using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdentityOneTable
{
    public class SQLiteOneIdentityOneTableStateStoreMigration : DbUpSQLiteMigration
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly ISQLiteOneIdentityOneTableStateStoreOptions _options;

        public SQLiteOneIdentityOneTableStateStoreMigration(
            IClaptrapIdentity identity,
            IClaptrapDesign claptrapDesign,
            IDbFactory dbFactory,
            ISQLiteOneIdentityOneTableStateStoreOptions options,
            ILogger<SQLiteOneIdentityOneTableStateStoreMigration> logger) : base(dbFactory, logger)
        {
            _identity = identity;
            _claptrapDesign = claptrapDesign;
            _options = options;
        }

        protected override string GetDbName()
        {
            return DbNameHelper.OneIdentityOneTableStateStore(_claptrapDesign, _identity);
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith("-OneIdentityOneTable.state.sql");
        }

        protected override Dictionary<string, string> GetVariables()
        {
            return new Dictionary<string, string>
            {
                {"StateTableName", _options.StateTableName}
            };
        }
    }
}