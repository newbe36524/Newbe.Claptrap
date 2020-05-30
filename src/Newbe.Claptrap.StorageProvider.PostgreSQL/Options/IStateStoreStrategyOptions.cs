namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public interface IStateStoreStrategyOptions
        : IStorageProviderOptions
    {
        PostgreSQLStateStoreStrategy PostgreSQLStateStoreStrategy { get; }
    }
}