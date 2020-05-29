using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IMySqlSharedTableStateStoreOptions :
        IMySqlStateLoaderOptions,
        IMySqlStateSaverOptions,
        IBatchEventSaverOptions,
        IMySqlMigrationOptions
    {
        string SchemaName { get; }
        string StateTableName { get; }
        string DbName { get; }
    }
}