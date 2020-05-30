namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public interface IEventStoreStrategyOptions
        : IStorageProviderOptions
    {
        PostgreSQLEventStoreStrategy PostgreSQLEventStoreStrategy { get; }
    }
}