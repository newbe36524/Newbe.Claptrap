namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IStrategyBaseStateStoreOptions
        : IStorageProviderOptions
    {
        StateStoreStrategy StateStoreStrategy { get; }
    }
}