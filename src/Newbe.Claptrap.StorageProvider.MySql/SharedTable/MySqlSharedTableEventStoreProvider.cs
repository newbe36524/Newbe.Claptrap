using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.RelationalDatabase;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.SharedTable
{
    public class MySqlSharedTableEventStoreProvider : ISharedTableEventStoreProvider
    {
        private readonly SharedEventTableDbMigrationManager.Factory _factory;
        private readonly IDbFactory _dbFactory;
        private readonly Lazy<string> _insertSql;
        private readonly Lazy<string> _selectSql;

        public MySqlSharedTableEventStoreProvider(
            SharedEventTableDbMigrationManager.Factory factory,
            IDbFactory dbFactory)
        {
            _factory = factory;
            _dbFactory = dbFactory;
            _insertSql = new Lazy<string>(() =>
                $"INSERT INTO [{SchemaName}].[{EventTableName}] ([claptrap_type_code], [claptrap_id], [version], [event_type_code], [event_data], [created_time]) VALUES (@ClaptrapTypeCode, @ClaptrapId, @Version, @EventTypeCode, @EventData, @CreatedTime)");

            _selectSql = new Lazy<string>(() =>
                $"SELECT * FROM [{SchemaName}].[{EventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]");
        }

        public async Task InsertOneAsync(SharedTableEventEntity entity)
        {
            var sql = _insertSql.Value;
            using var db =
                _dbFactory.GetConnection(TODO);
            await db.ExecuteAsync(sql, entity);
        }

        public Task InsertManyAsync(IEnumerable<SharedTableEventEntity> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SharedTableEventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var sql = _selectSql.Value;
            // todo db
            using var db =
                _dbFactory.GetConnection(TODO);
            var re = await db.QueryAsync<SharedTableEventEntity>(sql, new {startVersion, endVersion});
            return re;
        }

        public IDbMigrationManager DbMigrationManager(IClaptrapIdentity identity)
        {
            var manager = _factory.Invoke(identity, new SharedTableMigrationVar
            {
                SchemaName = SchemaName,
                EventTableName = EventTableName
            });
            return manager;
        }

        public string SchemaName { get; } = "claptrap";
        public string EventTableName { get; } = "claptrap_event_shared";
    }
}