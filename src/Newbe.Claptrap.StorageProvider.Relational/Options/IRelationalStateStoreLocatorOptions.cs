using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IRelationalStateStoreLocatorOptions
        : IStorageProviderOptions
    {
        IRelationalStateStoreLocator RelationalStateStoreLocator { get; }
    }
}