using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventStore
{
    public interface IEventStoreFactory
    {
        IEventStore Create(IActorIdentity identity);
    }
}