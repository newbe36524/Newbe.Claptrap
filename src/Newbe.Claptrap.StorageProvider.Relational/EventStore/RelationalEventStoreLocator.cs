using System;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class RelationalEventStoreLocator : IRelationalEventStoreLocator
    {
        private readonly Func<IClaptrapIdentity, string>? _connectionNameFunc;
        private readonly Func<IClaptrapIdentity, string>? _eventTableNameFunc;
        private readonly Func<IClaptrapIdentity, string>? _schemaNameFunc;
        private readonly string? _schemaName;
        private readonly string? _connectionName;
        private readonly string? _eventTableName;

        public RelationalEventStoreLocator(
            string? schemaName = null,
            string? connectionName = null,
            string? eventTableName = null,
            Func<IClaptrapIdentity, string>? schemaNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? eventTableNameFunc = null)
        {
            _schemaName = schemaName;
            _connectionName = connectionName;
            _eventTableName = eventTableName;
            _schemaNameFunc = schemaNameFunc;
            _connectionNameFunc = connectionNameFunc;
            _eventTableNameFunc = eventTableNameFunc;
        }

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return _connectionNameFunc?.Invoke(identity) ?? _connectionName!;
        }

        public string GetSchemaName(IClaptrapIdentity identity)
        {
            return _schemaNameFunc?.Invoke(identity) ?? _schemaName!;
        }

        public string GetEventTableName(IClaptrapIdentity identity)
        {
            return _eventTableNameFunc?.Invoke(identity) ?? _eventTableName!;
        }
    }
}