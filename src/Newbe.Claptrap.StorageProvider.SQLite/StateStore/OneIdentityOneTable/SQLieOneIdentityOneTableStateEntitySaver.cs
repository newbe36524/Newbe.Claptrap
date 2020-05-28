using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdentityOneTable
{
    public class SQLieOneIdentityOneTableStateEntitySaver :
        IStateEntitySaver<OneIdentityOneTableStateEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly IDbFactory _dbFactory;
        private readonly string _insertSql;

        public SQLieOneIdentityOneTableStateEntitySaver(
            IClaptrapIdentity claptrapIdentity,
            IClaptrapDesign claptrapDesign,
            IDbFactory dbFactory,
            ISqlTemplateCache sqlTemplateCache,
            ISQLiteOneIdentityOneTableStateStoreOptions stateStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _claptrapDesign = claptrapDesign;
            _dbFactory = dbFactory;
            var sql = sqlTemplateCache.Get(SQLiteSqlCacheKeys.OneIdentityOneTableStateStoreInsertOneSql);
            _insertSql = string.Format(sql, stateStoreOptions.StateTableName);
        }

        public async Task SaveAsync(OneIdentityOneTableStateEntity entity)
        {
            var dbName = DbNameHelper.OneIdentityOneTableStateStore(_claptrapDesign, _claptrapIdentity);
            using var db = _dbFactory.GetConnection(dbName);
            await db.ExecuteAsync(_insertSql, entity);
        }
    }
}