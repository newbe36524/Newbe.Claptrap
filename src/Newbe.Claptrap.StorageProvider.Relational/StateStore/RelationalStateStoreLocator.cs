using System;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class RelationalStateStoreLocator : IRelationalStateStoreLocator
    {
        private readonly Func<IClaptrapIdentity, string>? _connectionNameFunc;
        private readonly Func<IClaptrapIdentity, string>? _stateTableNameFunc;
        private readonly Func<IClaptrapIdentity, string>? _schemaNameFunc;
        private readonly string? _schemaName;
        private readonly string? _connectionName;
        private readonly string? _stateTableName;

        public RelationalStateStoreLocator(
            string? schemaName = null,
            string? connectionName = null,
            string? stateTableName = null,
            Func<IClaptrapIdentity, string>? schemaNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? stateTableNameFunc = null)
        {
            _schemaName = schemaName;
            _connectionName = connectionName;
            _stateTableName = stateTableName;
            _schemaNameFunc = schemaNameFunc;
            _connectionNameFunc = connectionNameFunc;
            _stateTableNameFunc = stateTableNameFunc;
        }

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return _connectionNameFunc?.Invoke(identity) ?? _connectionName!;
        }

        public string GetSchemaName(IClaptrapIdentity identity)
        {
            return _schemaNameFunc?.Invoke(identity) ?? _schemaName!;
        }

        public string GetStateTableName(IClaptrapIdentity identity)
        {
            return _stateTableNameFunc?.Invoke(identity) ?? _stateTableName!;
        }
    }
}