using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.SharedTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventEntitySaver : BatchEventEntitySaver<SharedTableEventEntity>
    {
        private readonly MySqlDatabaseConfig _mySqlDatabaseConfig;
        private readonly IDbFactory _dbFactory;
        private readonly Lazy<string> _insertSql;

        public MySqlSharedTableEventEntitySaver(
            EventSaverOptions eventSaverOptions,
            MySqlDatabaseConfig mySqlDatabaseConfig,
            IDbFactory dbFactory) : base(eventSaverOptions)
        {
            _mySqlDatabaseConfig = mySqlDatabaseConfig;
            var config = mySqlDatabaseConfig.SharedTableEventStoreConfig;
            _dbFactory = dbFactory;
            _insertSql = new Lazy<string>(() =>
                $"INSERT INTO [{config.SchemaName}].[{config.EventTableName}] ([claptrap_type_code], [claptrap_id], [version], [event_type_code], [event_data], [created_time]) VALUES (@ClaptrapTypeCode, @ClaptrapId, @Version, @EventTypeCode, @EventData, @CreatedTime)");
        }

        protected override async Task SaveOneAsync(SharedTableEventEntity entity)
        {
            var sql = _insertSql.Value;
            var dbName = _mySqlDatabaseConfig.SharedTableEventStoreConfig.SharedTableEventStoreDbName;
            using var db = _dbFactory.GetConnection(dbName);
            await db.ExecuteAsync(sql, entity);
        }

        protected override Task SaveManyAsync(IEnumerable<SharedTableEventEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}