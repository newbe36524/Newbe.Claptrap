using System;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class RelationalEventStoreLocator : IRelationalEventStoreLocator
    {
        public Func<IClaptrapIdentity, string>? ConnectionNameFunc { get; set; } = null!;
        public Func<IClaptrapIdentity, string>? EventTableNameFunc { get; set; } = null!;
        public Func<IClaptrapIdentity, string>? SchemaNameFunc { get; set; } = null!;
        public string? SchemaName { get; set; } = null!;
        public string? ConnectionName { get; set; } = null!;
        public string? EventTableName { get; set; } = null!;

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return ConnectionNameFunc?.Invoke(identity) ?? ConnectionName!;
        }

        public string GetSchemaName(IClaptrapIdentity identity)
        {
            return SchemaNameFunc?.Invoke(identity) ?? SchemaName!;
        }

        public string GetEventTableName(IClaptrapIdentity identity)
        {
            return EventTableNameFunc?.Invoke(identity) ?? EventTableName!;
        }
    }
}