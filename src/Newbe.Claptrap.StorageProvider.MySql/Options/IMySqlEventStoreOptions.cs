using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IMySqlEventStoreOptions :
        IMySqlEventLoaderOptions,
        IMySqlEventSaverOptions,
        IBatchSaverOptions,
        IRelationalEventStoreLocatorOptions,
        IMySqlMigrationOptions
    {
    }
}