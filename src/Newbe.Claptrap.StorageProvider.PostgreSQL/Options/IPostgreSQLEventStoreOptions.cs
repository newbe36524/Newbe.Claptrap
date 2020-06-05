using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public interface IPostgreSQLEventStoreOptions :
        IPostgreSQLEventLoaderOptions,
        IPostgreSQLEventSaverOptions,
        IBatchEventSaverOptions,
        IRelationalEventStoreLocatorOptions,
        IPostgreSQLMigrationOptions
    {
 
    }
}