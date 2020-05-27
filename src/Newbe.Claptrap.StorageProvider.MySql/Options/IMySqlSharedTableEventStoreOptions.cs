using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IMySqlSharedTableEventStoreOptions :
        IRelationalEventLoaderOptions,
        IRelationalEventSaverOptions,
        IBatchEventSaverOptions,
        IMySqlMigrationOptions
    {
        string SchemaName { get; }
        string EventTableName { get; }
        string SharedTableEventStoreDbName { get; }
    }
}