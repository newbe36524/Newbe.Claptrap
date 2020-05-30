using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileStateEntitySaver :
        IStateEntitySaver<StateEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly IDbFactory _isqLiteDbFactory;
        private readonly string _insertSql;

        public SQLiteOneIdOneFileStateEntitySaver(
            IClaptrapIdentity claptrapIdentity,
            IClaptrapDesign claptrapDesign,
            IDbFactory isqLiteDbFactory,
            ISQLiteOneIdOneFileStateStoreOptions stateStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _claptrapDesign = claptrapDesign;
            _isqLiteDbFactory = isqLiteDbFactory;
            _insertSql =
                $"INSERT OR REPLACE INTO [{stateStoreOptions.StateTableName}] ([claptrapid], [claptraptypecode], [version], [statedata],[updatedtime]) VALUES(@ClaptrapId, @ClaptrapTypeCode, @Version, @StateData, @UpdatedTime)";
        }

        public async Task SaveAsync(StateEntity entity)
        {
            var dbName = SQLiteDbNameHelper.OneIdOneFileStateStore(_claptrapDesign, _claptrapIdentity);
            using var db = _isqLiteDbFactory.GetConnection(dbName);
            await db.ExecuteAsync(_insertSql, entity);
        }
    }
}