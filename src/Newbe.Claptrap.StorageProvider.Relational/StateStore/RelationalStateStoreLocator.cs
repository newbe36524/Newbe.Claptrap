using System;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class RelationalStateStoreLocator : IRelationalStateStoreLocator
    {
        public Func<IClaptrapIdentity, string>? ConnectionNameFunc { get; set; } = null!;
        public Func<IClaptrapIdentity, string>? StateTableNameFunc { get; set; } = null!;
        public Func<IClaptrapIdentity, string>? SchemaNameFunc { get; set; } = null!;
        public string? SchemaName { get; set; } = null!;
        public string? ConnectionName { get; set; } = null!;
        public string? StateTableName { get; set; } = null!;

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return ConnectionNameFunc?.Invoke(identity) ?? ConnectionName!;
        }

        public string GetSchemaName(IClaptrapIdentity identity)
        {
            return SchemaNameFunc?.Invoke(identity) ?? SchemaName!;
        }

        public string GetStateTableName(IClaptrapIdentity identity)
        {
            return StateTableNameFunc?.Invoke(identity) ?? StateTableName!;
        }
    }
}