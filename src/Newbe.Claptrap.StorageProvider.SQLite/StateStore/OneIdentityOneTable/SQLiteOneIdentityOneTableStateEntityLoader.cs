using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdentityOneTable
{
    public class SQLiteOneIdentityOneTableStateEntityLoader
        : IStateEntityLoader<OneIdentityOneTableStateEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly IDbFactory _dbFactory;
        private readonly string _selectSql;

        public SQLiteOneIdentityOneTableStateEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            IClaptrapDesign claptrapDesign,
            IDbFactory dbFactory,
            ISqlTemplateCache sqlTemplateCache,
            ISQLiteOneIdentityOneTableStateStoreOptions stateStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _claptrapDesign = claptrapDesign;
            _dbFactory = dbFactory;
            var sql = sqlTemplateCache.Get(SQLiteSqlCacheKeys.OneIdentityOneTableStateStoreSelectSql);
            _selectSql = string.Format(sql, stateStoreOptions.StateTableName);
        }

        public async Task<OneIdentityOneTableStateEntity> GetStateSnapshotAsync()
        {
            var dbName = DbNameHelper.OneIdentityOneTableStateStore(_claptrapDesign, _claptrapIdentity);
            using var db = _dbFactory.GetConnection(dbName);
            var ps = new {ClaptrapTypeCode = _claptrapIdentity.TypeCode, ClaptrapId = _claptrapIdentity.Id};
            var re = await db.QueryFirstOrDefaultAsync<OneIdentityOneTableStateEntity>(_selectSql, ps);
            return re;
        }
    }
}