using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventStore
{
    public interface IEventStoreFactory
    {
        IEventStore Create(IActorIdentity identity);
    }
}