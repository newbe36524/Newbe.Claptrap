using System;
using Newbe.Claptrap.StorageProvider.RelationalDatabase;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.SharedTable
{
    public class MySqlShareTableEventStoreProvider : IShareTableEventStoreProvider
    {
        private readonly SharedEventTableDbMigrationManager.Factory _factory;
        private readonly Lazy<string> _insertSql;
        private readonly Lazy<string> _selectSql;

        public MySqlShareTableEventStoreProvider(
            SharedEventTableDbMigrationManager.Factory factory)
        {
            _factory = factory;
            _insertSql = new Lazy<string>(() =>
                $"INSERT INTO [{SchemaName}].[{EventTableName}] ([claptrap_type_code], [claptrap_id], [version], [event_type_code], [event_data], [created_time]) VALUES (@ClaptrapTypeCode, @ClaptrapId, @Version, @EventTypeCode, @EventData, @CreatedTime)");

            _selectSql = new Lazy<string>(() =>
                $"SELECT * FROM [{SchemaName}].[{EventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]");
        }

        public string CreateInsertOneSql(IClaptrapIdentity identity)
        {
            return _insertSql.Value;
        }

        public string CreateSelectSql(IClaptrapIdentity identity)
        {
            return _selectSql.Value;
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