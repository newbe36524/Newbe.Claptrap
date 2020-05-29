using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IMySqlSharedTableEventStoreOptions :
        IMySqlEventLoaderOptions,
        IMySqlEventSaverOptions,
        IBatchEventSaverOptions,
        IMySqlMigrationOptions
    {
        string SchemaName { get; }
        string EventTableName { get; }
        string DbName { get; }
    }
}