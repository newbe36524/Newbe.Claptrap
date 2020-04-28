using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventStore
{
    public interface IIEventStoreFactoryHandler
    {
        IEventStore Create(IActorIdentity identity);
    }
}