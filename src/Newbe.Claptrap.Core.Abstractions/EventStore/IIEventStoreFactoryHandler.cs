using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventStore
{
    public interface IIEventStoreFactoryHandler
    {
        IEventStore Create(IActorIdentity identity);
    }
}