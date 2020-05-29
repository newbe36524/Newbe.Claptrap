using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileStateStoreMigration : DbUpSQLiteMigration
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly ISQLiteOneIdOneFileStateStoreOptions _options;

        public SQLiteOneIdOneFileStateStoreMigration(
            IClaptrapIdentity identity,
            IClaptrapDesign claptrapDesign,
            IDbFactory dbFactory,
            ISQLiteOneIdOneFileStateStoreOptions options,
            ILogger<SQLiteOneIdOneFileStateStoreMigration> logger) : base(dbFactory, logger)
        {
            _identity = identity;
            _claptrapDesign = claptrapDesign;
            _options = options;
        }

        protected override string GetDbName()
        {
            return SQLiteDbNameHelper.OneIdentityOneTableStateStore(_claptrapDesign, _identity);
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith("-OneIdOneFile.state.sql");
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