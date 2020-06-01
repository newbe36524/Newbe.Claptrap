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
                $"INSERT OR REPLACE INTO [{stateStoreOptions.StateTableName}] ([claptrap_id], [claptrap_type_code], [version], [state_data],[updated_time]) VALUES(@claptrap_id, @claptrap_type_code, @version, @state_data, @updated_time)";
        }

        public async Task SaveAsync(StateEntity entity)
        {
            var connectionName = SQLiteConnectionNameHelper.OneIdOneFileStateStore(_claptrapDesign, _claptrapIdentity);
            using var db = _isqLiteDbFactory.GetConnection(connectionName);
            var item = new OneIdOneFileStateEntity
            {
                claptrap_id = entity.ClaptrapId,
                claptrap_type_code = entity.ClaptrapTypeCode,
                state_data = entity.StateData,
                updated_time = entity.UpdatedTime,
                version = entity.Version
            };
            await db.ExecuteAsync(_insertSql, item);
        }
    }
}