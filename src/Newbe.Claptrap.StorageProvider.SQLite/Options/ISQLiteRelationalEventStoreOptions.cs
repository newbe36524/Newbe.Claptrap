using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.SQLite.Options.Core;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteRelationalEventStoreOptions :
        ISQLiteEventLoaderOptions,
        ISQLiteEventSaverOptions,
        IBatchEventSaverOptions,
        ISQLiteStorageMigrationOptions,
        IRelationalEventStoreLocatorOptions
    {
    }
}