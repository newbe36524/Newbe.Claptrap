using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.Options
{
    public interface IRelationDatabaseEventLoaderOptions :
        IEventLoaderOptions
    {
        EventStoreStrategy EventStoreStrategy { get; }
    }
}