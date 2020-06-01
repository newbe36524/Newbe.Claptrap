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
        private readonly IDbFactory _isqLiteDbFactory;
        private readonly string _insertSql;

        public SQLiteOneIdOneFileEventEntitySaver(
            IClaptrapIdentity claptrapIdentity,
            IDbFactory isqLiteDbFactory,
            ISQLiteOneIdOneFileEventStoreOptions eventStoreOptions)
        {
            _claptrapIdentity = claptrapIdentity;
            _isqLiteDbFactory = isqLiteDbFactory;
            _insertSql =
                $"INSERT INTO [{eventStoreOptions.EventTableName}] ([version], [event_type_code], [event_data], [created_time]) VALUES (@version, @event_type_code, @event_data, @created_time)";
        }

        public async Task SaveAsync(EventEntity entity)
        {
            var connectionName = SQLiteConnectionNameHelper.OneIdOneFileEventStore(_claptrapIdentity);
            using var db = _isqLiteDbFactory.GetConnection(connectionName);
            var item = new OneIdOneFileEventEntity
            {
                created_time = entity.CreatedTime,
                event_data = entity.EventData,
                event_type_code = entity.EventTypeCode,
                version = entity.Version
            };
            await db.ExecuteAsync(_insertSql, item);
        }
    }
}