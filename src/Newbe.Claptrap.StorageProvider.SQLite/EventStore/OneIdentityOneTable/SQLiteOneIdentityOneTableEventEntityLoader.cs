using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public class SQLiteOneIdentityOneTableEventEntityLoader
        : IEventEntityLoader<OneIdentityOneTableEventEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly IDbFactory _dbFactory;
        private readonly Lazy<string> _selectSql;

        public SQLiteOneIdentityOneTableEventEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            IClaptrapDesign claptrapDesign,
            IDbFactory dbFactory,
            ISqlTemplateCache sqlTemplateCache,
            ISQLiteOneIdentityOneTableEventStoreOptions eventStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _claptrapDesign = claptrapDesign;
            _dbFactory = dbFactory;
            var sql = sqlTemplateCache.Get(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreSelectSql);
            _selectSql = new Lazy<string>(() => string.Format(sql, eventStoreOptions.EventTableName));
        }

        public async Task<IEnumerable<OneIdentityOneTableEventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var dbName = DbNameHelper.GetDbNameForOneIdentityOneTable(_claptrapDesign, _claptrapIdentity);
            using var db = _dbFactory.GetConnection(dbName);
            var sql = _selectSql.Value;
            var re = await db.QueryAsync<OneIdentityOneTableEventEntity>(sql, new {startVersion, endVersion});
            return re.ToArray();
        }
    }
}