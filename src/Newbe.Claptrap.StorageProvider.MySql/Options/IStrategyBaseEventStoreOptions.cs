namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IStrategyBaseEventStoreOptions
        : IStorageProviderOptions
    {
        EventStoreStrategy EventStoreStrategy { get; }
    }
}