namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IStateStoreStrategyOptions
        : IStorageProviderOptions
    {
        MySqlStateStoreStrategy MySqlStateStoreStrategy { get; }
    }
}