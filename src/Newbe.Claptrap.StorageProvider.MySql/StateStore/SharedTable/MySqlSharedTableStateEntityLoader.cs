using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.StateStore.SharedTable
{
    public class MySqlSharedTableStateEntityLoader
        : IStateEntityLoader<StateEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IDbFactory _dbFactory;
        private readonly IMySqlSharedTableStateStoreOptions _options;
        private readonly string _selectSql;

        public MySqlSharedTableStateEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            IDbFactory dbFactory,
            IMySqlSharedTableStateStoreOptions options)
        {
            _claptrapIdentity = claptrapIdentity;
            _dbFactory = dbFactory;
            _options = options;
            _selectSql =
                $"SELECT * FROM {options.SchemaName}.{options.StateTableName} WHERE claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId LIMIT 1";
        }

        public async Task<StateEntity?> GetStateSnapshotAsync()
        {
            using var db = _dbFactory.GetConnection(_options.DbName);
            var ps = new {ClaptrapTypeCode = _claptrapIdentity.TypeCode, ClaptrapId = _claptrapIdentity.Id};
            var item = await db.QueryFirstOrDefaultAsync<SharedTableStateEntity>(_selectSql, ps);
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