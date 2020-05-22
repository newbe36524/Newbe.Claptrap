using System;

namespace Newbe.Claptrap.StorageProvider.MySql.SharedTable
{
    public class MySqlShareTableStateStoreProvider
    {
        private readonly Lazy<string> _selectSql;
        private readonly Lazy<string> _upsertSql;

        public MySqlShareTableStateStoreProvider()
        {
            _selectSql = new Lazy<string>(() =>
                $"SELECT * FROM [{SchemaName}].[{StateTableName}] WHERE [claptrap_type_code]=@ClaptrapTypeCode AND [claptrap_id]=@ClaptrapId LIMIT 1");
            _upsertSql = new Lazy<string>(() =>
                $"REPLACE INTO [{SchemaName}].[{StateTableName}] ([claptrap_type_code],[claptrap_id],[version],[state_data],[updated_time]) VALUES(@ClaptrapTypeCode, @ClaptrapId, @Version, @StateData, @UpdatedTime)");
        }

        public string CreateUpsertSql(IClaptrapIdentity identity)
        {
            return _upsertSql.Value;
        }

        public string CreateSelectSql(IClaptrapIdentity identity)
        {
            return _selectSql.Value;
        }

        public string SchemaName { get; } = "claptrap";
        public string StateTableName { get; } = "claptrap_state_shared";
    }
}