using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Options
{
    public interface IPostgreSQLSharedTableStateStoreOptions :
        IPostgreSQLStateLoaderOptions,
        IPostgreSQLStateSaverOptions,
        IBatchEventSaverOptions,
        IPostgreSQLMigrationOptions
    {
        string SchemaName { get; }
        string StateTableName { get; }
        string DbName { get; }
    }
}