using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.Autofac
{
    public class EventStoreFactory : IEventStoreFactory
    {
        public IEventStore Create(IActorIdentity identity)
        {
            // TODO impl
            return new MemoryEventStore(identity);
        }
    }
}