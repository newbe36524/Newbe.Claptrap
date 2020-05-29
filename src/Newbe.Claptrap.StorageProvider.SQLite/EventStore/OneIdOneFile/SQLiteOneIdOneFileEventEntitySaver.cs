using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdOneFile
{
    public class SQLiteOneIdOneFileEventEntitySaver :
        IEventEntitySaver<EventEntity>
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IClaptrapDesign _claptrapDesign;
        private readonly IDbFactory _isqLiteDbFactory;
        private readonly string _insertSql;

        public SQLiteOneIdOneFileEventEntitySaver(
            IClaptrapIdentity claptrapIdentity,
            IClaptrapDesign claptrapDesign,
            IDbFactory isqLiteDbFactory,
            ISQLiteOneIdOneFileEventStoreOptions eventStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _claptrapDesign = claptrapDesign;
            _isqLiteDbFactory = isqLiteDbFactory;
            _insertSql =
                $"INSERT INTO [{eventStoreOptions.EventTableName}] ([version], [eventtypecode], [eventdata], [createdtime]) VALUES (@Version, @EventTypeCode, @EventData, @CreatedTime)";
        }

        public async Task SaveAsync(EventEntity entity)
        {
            var dbName = SQLiteDbNameHelper.OneIdentityOneTableEventStore(_claptrapDesign, _claptrapIdentity);
            using var db = _isqLiteDbFactory.GetConnection(dbName);
            await db.ExecuteAsync(_insertSql, entity);
        }
    }
}