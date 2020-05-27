using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteOneIdentityOneTableEventStoreOptions :
        IRelationalEventLoaderOptions,
        IRelationalEventSaverOptions,
        ISQLiteStorageMigrationOptions
    {
        string EventTableName { get; }
    }
}