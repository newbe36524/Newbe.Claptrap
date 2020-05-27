using System;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public class SQLieOneIdentityOneTableEventEntitySaver :
        IEventEntitySaver<OneIdentityOneTableEventEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IDbFactory _dbFactory;
        private readonly Lazy<string> _insertSql;

        public SQLieOneIdentityOneTableEventEntitySaver(
            IClaptrapIdentity claptrapIdentity,
            IDbFactory dbFactory,
            ISqlCache sqlCache,
            ISQLiteOneIdentityOneTableEventStoreOptions eventStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _dbFactory = dbFactory;
            var sql = sqlCache.Get(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreInsertOneSql);
            _insertSql = new Lazy<string>(() => sql.Replace("{eventTableName}", eventStoreOptions.EventTableName));
        }

        public async Task SaveAsync(OneIdentityOneTableEventEntity entity)
        {
            var sql = _insertSql.Value;
            var dbName = DbNameHelper.GetDbNameForOneIdentityOneTable(_claptrapIdentity);
            using var db = _dbFactory.GetConnection(dbName);
            await db.ExecuteAsync(sql, entity);
        }
    }
}