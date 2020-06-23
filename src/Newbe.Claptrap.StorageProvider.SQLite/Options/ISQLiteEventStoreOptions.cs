using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteEventStoreOptions :
        ISQLiteEventLoaderOptions,
        ISQLiteEventSaverOptions,
        IBatchSaverOptions,
        ISQLiteStorageMigrationOptions,
        IRelationalEventStoreLocatorOptions
    {
    }
}