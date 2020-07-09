using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.Relational.Extensions
{
    public static class RelationalEventStoreLocatorExtensions
    {
        public static (string connectionName, string schemaName, string eventTableName)
            GetNames(this IRelationalEventStoreLocator locator, IClaptrapIdentity identity)
        {
            return (locator.GetConnectionName(identity), locator.GetSchemaName(identity),
                locator.GetEventTableName(identity));
        }

        public static (string connectionName, string schemaName, string stateTableName)
            GetNames(this IRelationalStateStoreLocator locator, IClaptrapIdentity identity)
        {
            return (locator.GetConnectionName(identity), locator.GetSchemaName(identity),
                locator.GetStateTableName(identity));
        }
    }
}