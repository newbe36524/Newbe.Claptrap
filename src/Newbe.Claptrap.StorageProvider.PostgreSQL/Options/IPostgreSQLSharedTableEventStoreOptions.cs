using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public interface IPostgreSQLSharedTableEventStoreOptions :
        IPostgreSQLEventLoaderOptions,
        IPostgreSQLEventSaverOptions,
        IBatchEventSaverOptions,
        IPostgreSQLMigrationOptions
    {
        string SchemaName { get; }
        string EventTableName { get; }
        string ConnectionName { get; }
    }
}