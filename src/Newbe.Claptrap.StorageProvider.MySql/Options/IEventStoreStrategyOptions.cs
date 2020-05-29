namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IEventStoreStrategyOptions
        : IStorageProviderOptions
    {
        MySqlEventStoreStrategy MySqlEventStoreStrategy { get; }
    }
}