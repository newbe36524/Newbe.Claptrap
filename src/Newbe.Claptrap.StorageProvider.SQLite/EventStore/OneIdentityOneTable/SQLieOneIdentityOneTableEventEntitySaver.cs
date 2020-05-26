using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public class SQLieOneIdentityOneTableEventEntitySaver :
        IEventEntitySaver<OneIdentityOneTableEventEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IDbFactory _dbFactory;
        private readonly ISqlCache _sqlCache;

        public SQLieOneIdentityOneTableEventEntitySaver(
            IClaptrapIdentity claptrapIdentity,
            IDbFactory dbFactory,
            ISqlCache sqlCache)
        {
            _claptrapIdentity = claptrapIdentity;
            _dbFactory = dbFactory;
            _sqlCache = sqlCache;
        }

        public async Task SaveAsync(OneIdentityOneTableEventEntity entity)
        {
            var sql = _sqlCache.Get(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreInsertOneSql);
            var dbName = DbNameHelper.GetDbNameForOneIdentityOneTable(_claptrapIdentity);
            using var db = _dbFactory.GetConnection(dbName);
            await db.ExecuteAsync(sql, entity);
        }
    }
}