using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IMySqlStateStoreOptions :
        IMySqlStateLoaderOptions,
        IMySqlStateSaverOptions,
        IBatchEventSaverOptions,
        IRelationalStateStoreLocatorOptions,
        IMySqlMigrationOptions
    {
    }
}