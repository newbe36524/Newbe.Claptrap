using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.Options
{
    public interface IRelationDatabaseEventSaverOptions :
        IEventSaverOptions
    {
        EventStoreStrategy EventStoreStrategy { get; }
    }
}