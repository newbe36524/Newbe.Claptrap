using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IStrategyBaseStateStoreOptions
        : IStorageProviderOptions
    {
        StateStoreStrategy StateStoreStrategy { get; }
    }
}