using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public class SQLiteOneIdentityOneTableEventEntityLoader
        : IEventEntityLoader<OneIdentityOneTableEventEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IDbFactory _dbFactory;
        private readonly ISqlCache _sqlCache;

        public SQLiteOneIdentityOneTableEventEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            IDbFactory dbFactory,
            ISqlCache sqlCache)
        {
            _claptrapIdentity = claptrapIdentity;
            _dbFactory = dbFactory;
            _sqlCache = sqlCache;
        }

        public async Task<IEnumerable<OneIdentityOneTableEventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var dbName = DbNameHelper.GetDbNameForOneIdentityOneTable(_claptrapIdentity);
            using var db = _dbFactory.GetConnection(dbName);
            var sql = _sqlCache.Get(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreSelectSql);
            var re = await db.QueryAsync<OneIdentityOneTableEventEntity>(sql, new {startVersion, endVersion});
            return re.ToArray();
        }
    }
}