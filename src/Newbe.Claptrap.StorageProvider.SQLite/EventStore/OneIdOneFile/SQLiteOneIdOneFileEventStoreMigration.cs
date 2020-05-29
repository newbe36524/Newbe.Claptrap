using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileEventStoreMigration : DbUpSQLiteMigration
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly ISQLiteOneIdOneFileEventStoreOptions _options;

        public SQLiteOneIdOneFileEventStoreMigration(
            IClaptrapIdentity identity,
            IClaptrapDesign claptrapDesign,
            ISQLiteOneIdOneFileEventStoreOptions options,
            IDbFactory dbFactory,
            ILogger<SQLiteOneIdOneFileEventStoreMigration> logger)
            : base(dbFactory, logger)
        {
            _identity = identity;
            _claptrapDesign = claptrapDesign;
            _options = options;
        }

        protected override string GetDbName()
        {
            return SQLiteDbNameHelper.OneIdentityOneTableEventStore(_claptrapDesign, _identity);
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith("-OneIdOneFile.event.sql");
        }

        protected override Dictionary<string, string> GetVariables()
        {
            var ps = new Dictionary<string, string>
            {
                {"EventTableName", _options.EventTableName},
            };
            return ps;
        }
    }
}