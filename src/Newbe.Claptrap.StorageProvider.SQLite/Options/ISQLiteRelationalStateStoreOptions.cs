using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.SQLite.Options.Core;

namespace Newbe.Claptrap.StorageProvider.SQLite.Options
{
    public interface ISQLiteRelationalStateStoreOptions :
        ISQLiteStateLoaderOptions,
        ISQLiteStateSaverOptions,
        ISQLiteStorageMigrationOptions,
        IRelationalStateStoreLocatorOptions
    {
    }
}