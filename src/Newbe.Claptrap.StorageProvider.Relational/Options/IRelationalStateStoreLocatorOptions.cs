using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IRelationalStateStoreLocatorOptions
        : IBatchSaverOptions
    {
        IRelationalStateStoreLocator RelationalStateStoreLocator { get; }
    }
}