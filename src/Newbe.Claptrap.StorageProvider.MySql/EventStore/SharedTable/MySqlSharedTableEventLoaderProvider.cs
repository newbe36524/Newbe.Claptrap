using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.MySql.SharedTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class MySqlSharedTableEventLoaderProvider : IEventEntityLoader<SharedTableEventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly MySqlDatabaseConfig _mySqlDatabaseConfig;
        private readonly Lazy<string> _selectSql;

        public MySqlSharedTableEventLoaderProvider(
            IDbFactory dbFactory,
            MySqlDatabaseConfig mySqlDatabaseConfig)
        {
            _dbFactory = dbFactory;
            _mySqlDatabaseConfig = mySqlDatabaseConfig;
            var config = mySqlDatabaseConfig.SharedTableEventStoreConfig;
            _selectSql = new Lazy<string>(() =>
                $"SELECT * FROM [{config.SchemaName}].[{config.EventTableName}] WHERE [version] >= @startVersion AND [version] < @endVersion ORDER BY [version]");
        }

        public async Task<IEnumerable<SharedTableEventEntity>> SelectAsync(long startVersion, long endVersion)
        {
            var dbName = _mySqlDatabaseConfig.SharedTableEventStoreConfig.SharedTableEventStoreDbName;
            using var db = _dbFactory.GetConnection(dbName);
            var sql = _selectSql.Value;
            var re = await db.QueryAsync<SharedTableEventEntity>(sql, new {startVersion, endVersion});
            return re.ToArray();
        }
    }
}