using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore.SharedTable
{
    public class PostgreSQLSharedTableStateEntityLoader
        : IStateEntityLoader<StateEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IDbFactory _dbFactory;
        private readonly IPostgreSQLSharedTableStateStoreOptions _options;
        private readonly string _selectSql;

        public PostgreSQLSharedTableStateEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            IDbFactory dbFactory,
            IPostgreSQLSharedTableStateStoreOptions options)
        {
            _claptrapIdentity = claptrapIdentity;
            _dbFactory = dbFactory;
            _options = options;
            _selectSql =
                $"SELECT * FROM {options.SchemaName}.{options.StateTableName} WHERE claptrap_type_code=@ClaptrapTypeCode AND claptrap_id=@ClaptrapId LIMIT 1";
        }

        public async Task<StateEntity?> GetStateSnapshotAsync()
        {
            using var db = _dbFactory.GetConnection(_options.ConnectionName);
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