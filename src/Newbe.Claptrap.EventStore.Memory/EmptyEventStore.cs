using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventStore.Memory
{
    /// <inheritdoc />
    /// <summary>
    /// this is using for test 
    /// </summary>
    public class EmptyEventStore : IEventStore
    {
        public IActorIdentity Identity { get; }

        public EmptyEventStore(IActorIdentity identity)
        {
            Identity = identity;
        }

        public Task<EventSavingResult> SaveEvent(IEvent @event)
        {
            return Task.FromResult(EventSavingResult.Success);
        }

        public Task<IEnumerable<IEvent>> GetEvents(ulong startVersion, ulong endVersion)
        {
            return Task.FromResult(Enumerable.Empty<IEvent>());
        }
    }
}