using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public class OneIdentityOneTableDbUpSQLiteMigration : DbUpSQLiteMigration
    {
        public delegate OneIdentityOneTableDbUpSQLiteMigration Factory(IClaptrapIdentity identity,
            ISQLiteOneIdentityOneTableEventStoreOptions options);

        private readonly IClaptrapIdentity _identity;
        private readonly ISQLiteOneIdentityOneTableEventStoreOptions _options;

        public OneIdentityOneTableDbUpSQLiteMigration(
            IClaptrapIdentity identity,
            ISQLiteOneIdentityOneTableEventStoreOptions options,
            IDbFactory dbFactory,
            ILogger<OneIdentityOneTableDbUpSQLiteMigration> logger)
            : base(dbFactory, logger)
        {
            _identity = identity;
            _options = options;
        }

        protected override string GetDbName()
        {
            return DbNameHelper.GetDbNameForOneIdentityOneTable(_identity);
        }

        protected override bool SqlSelector(string fileName)
        {
            return fileName.EndsWith(".event.sql");
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