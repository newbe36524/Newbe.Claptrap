using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.SQLite.Options.Core;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteSharedTableEventStoreOptions :
        ISQLiteEventLoaderOptions,
        ISQLiteEventSaverOptions,
        IBatchEventSaverOptions,
        ISQLiteStorageMigrationOptions
    {
        string ConnectionName { get; }
        string EventTableName { get; }
    }
}