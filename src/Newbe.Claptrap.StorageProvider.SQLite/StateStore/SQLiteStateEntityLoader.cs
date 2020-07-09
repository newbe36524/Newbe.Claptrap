using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.StateStore
{
    public class SQLiteStateEntityLoader
        : IStateEntityLoader<StateEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly ISQLiteDbFactory _sqLiteDbFactory;
        private readonly string _selectSql;
        private readonly string _connectionName;

        public SQLiteStateEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            ISQLiteDbFactory sqLiteDbFactory,
            ISQLiteStateStoreOptions options)
        {
            _claptrapIdentity = claptrapIdentity;
            _sqLiteDbFactory = sqLiteDbFactory;
            var locator = options.RelationalStateStoreLocator;
            var stateTableName = locator.GetStateTableName(claptrapIdentity);
            _connectionName = locator.GetConnectionName(claptrapIdentity);
            _selectSql =
                $"SELECT * FROM {stateTableName} WHERE claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId LIMIT 1";
        }

        public async Task<StateEntity?> GetStateSnapshotAsync()
        {
            using var db = _sqLiteDbFactory.GetConnection(_connectionName);
            var ps = new {ClaptrapTypeCode = _claptrapIdentity.TypeCode, ClaptrapId = _claptrapIdentity.Id};
            var item = await db.QueryFirstOrDefaultAsync<RelationalStateEntity>(_selectSql, ps);
            if (item == null)
            {
                return null;
            }

            var re = new StateEntity
            {
                Version = item.version,
                ClaptrapId = item.claptrap_id,
                StateData = item.state_data,
                UpdatedTime = item.updated_time,
                ClaptrapTypeCode = item.claptrap_type_code,
            };
            return re;
        }
    }
}