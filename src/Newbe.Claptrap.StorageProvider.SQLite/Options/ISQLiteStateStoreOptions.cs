using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteStateStoreOptions :
        ISQLiteStateLoaderOptions,
        ISQLiteStateSaverOptions,
        ISQLiteStorageMigrationOptions,
        IRelationalStateStoreLocatorOptions
    {
    }
}