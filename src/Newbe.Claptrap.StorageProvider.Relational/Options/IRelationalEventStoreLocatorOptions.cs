using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IRelationalEventStoreLocatorOptions
        : IStorageProviderOptions
    {
        IRelationalEventStoreLocator RelationalEventStoreLocator { get; }
    }
}