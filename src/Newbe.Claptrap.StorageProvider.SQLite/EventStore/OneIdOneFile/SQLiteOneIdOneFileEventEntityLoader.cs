using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileEventEntityLoader
        : IEventEntityLoader<EventEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly IDbFactory _isqLiteDbFactory;
        private readonly string _selectSql;

        public SQLiteOneIdOneFileEventEntityLoader(
            IClaptrapIdentity claptrapIdentity,
            IClaptrapDesign claptrapDesign,
            IDbFactory isqLiteDbFactory,
            ISQLiteOneIdOneFileEventStoreOptions eventStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _claptrapDesign = claptrapDesign;
            _isqLiteDbFactory = isqLiteDbFactory;
            _selectSql =
                $"SELECT * FROM [{eventStoreOptions.EventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]";
        }

        public async Task<IEnumerable<EventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var dbName = SQLiteDbNameHelper.OneIdentityOneTableEventStore(_claptrapDesign, _claptrapIdentity);
            using var db = _isqLiteDbFactory.GetConnection(dbName);
            var eventEntities = await db.QueryAsync<EventEntity>(_selectSql, new {startVersion, endVersion});
            var re = eventEntities.ToArray();
            foreach (var eventEntity in re)
            {
                eventEntity.ClaptrapId = _claptrapIdentity.Id;
                eventEntity.ClaptrapTypeCode = _claptrapIdentity.TypeCode;
            }

            return re;
        }
    }
}