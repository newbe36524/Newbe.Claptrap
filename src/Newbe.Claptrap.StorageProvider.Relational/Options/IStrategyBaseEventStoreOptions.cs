using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IStrategyBaseEventStoreOptions
        : IStorageProviderOptions
    {
        EventStoreStrategy EventStoreStrategy { get; }
    }
}