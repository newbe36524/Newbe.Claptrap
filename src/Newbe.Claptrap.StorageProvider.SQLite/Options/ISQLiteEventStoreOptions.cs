using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteEventStoreOptions :
        ISQLiteEventLoaderOptions,
        ISQLiteEventSaverOptions,
        IBatchEventSaverOptions,
        ISQLiteStorageMigrationOptions,
        IRelationalEventStoreLocatorOptions
    {
    }
}