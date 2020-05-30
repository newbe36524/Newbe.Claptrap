using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileStateEntityLoader
        : IStateEntityLoader<StateEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly IDbFactory _isqLiteDbFactory;
        private readonly string _selectSql;

        public SQLiteOneIdOneFileStateEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            IClaptrapDesign claptrapDesign,
            IDbFactory isqLiteDbFactory,
            ISQLiteOneIdOneFileStateStoreOptions stateStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _claptrapDesign = claptrapDesign;
            _isqLiteDbFactory = isqLiteDbFactory;
            _selectSql =
                $"SELECT * FROM [{stateStoreOptions.StateTableName}] WHERE [claptraptypecode]=@ClaptrapTypeCode AND [claptrapid]=@ClaptrapId LIMIT 1";
        }

        public async Task<StateEntity?> GetStateSnapshotAsync()
        {
            var dbName = SQLiteDbNameHelper.OneIdOneFileStateStore(_claptrapDesign, _claptrapIdentity);
            using var db = _isqLiteDbFactory.GetConnection(dbName);
            var ps = new {ClaptrapTypeCode = _claptrapIdentity.TypeCode, ClaptrapId = _claptrapIdentity.Id};
            var re = await db.QueryFirstOrDefaultAsync<StateEntity>(_selectSql, ps);
            if (re == null)
            {
                return null;
            }

            re.ClaptrapId = _claptrapIdentity.Id;
            re.ClaptrapTypeCode = _claptrapIdentity.TypeCode;
            return re;
        }
    }
}