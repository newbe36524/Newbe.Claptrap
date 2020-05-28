using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public class SQLiteOneIdentityOneTableEventStoreMigration : DbUpSQLiteMigration
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly ISQLiteOneIdentityOneTableEventStoreOptions _options;

        public SQLiteOneIdentityOneTableEventStoreMigration(
            IClaptrapIdentity identity,
            IClaptrapDesign claptrapDesign,
            ISQLiteOneIdentityOneTableEventStoreOptions options,
            IDbFactory dbFactory,
            ILogger<SQLiteOneIdentityOneTableEventStoreMigration> logger)
            : base(dbFactory, logger)
        {
            _identity = identity;
            _claptrapDesign = claptrapDesign;
            _options = options;
        }

        protected override string GetDbName()
        {
            return DbNameHelper.OneIdentityOneTableEventStore(_claptrapDesign, _identity);
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith("-OneIdentityOneTable.event.sql");
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