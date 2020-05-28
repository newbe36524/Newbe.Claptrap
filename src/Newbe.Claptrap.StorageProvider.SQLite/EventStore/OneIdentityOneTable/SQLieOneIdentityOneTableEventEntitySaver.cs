using System;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options;
using SmartFormat;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public class SQLieOneIdentityOneTableEventEntitySaver :
        IEventEntitySaver<OneIdentityOneTableEventEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly IDbFactory _dbFactory;
        private readonly Lazy<string> _insertSql;

        public SQLieOneIdentityOneTableEventEntitySaver(
            IClaptrapIdentity claptrapIdentity,
            IClaptrapDesign claptrapDesign,
            IDbFactory dbFactory,
            ISqlTemplateCache sqlTemplateCache,
            ISQLiteOneIdentityOneTableEventStoreOptions eventStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _claptrapDesign = claptrapDesign;
            _dbFactory = dbFactory;
            var sql = sqlTemplateCache.Get(SQLiteSqlCacheKeys.OneIdentityOneTableEventStoreInsertOneSql);
            _insertSql = new Lazy<string>(() => string.Format(sql, eventStoreOptions.EventTableName));
        }

        public async Task SaveAsync(OneIdentityOneTableEventEntity entity)
        {
            var sql = _insertSql.Value;
            var dbName = DbNameHelper.GetDbNameForOneIdentityOneTable(_claptrapDesign, _claptrapIdentity);
            using var db = _dbFactory.GetConnection(dbName);
            await db.ExecuteAsync(sql, entity);
        }
    }
}