using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventStore.Memory
{
    public class MemoryEventStore : IEventStore
    {
        private readonly IList<IEvent> _list = new List<IEvent>();

        public MemoryEventStore(IActorIdentity identity)
        {
            Identity = identity;
        }


        public IActorIdentity Identity { get; }

        public Task<EventSavingResult> SaveEvent(IEvent @event)
        {
            if (@event.Uid != null)
            {
                if (_list.Any(x => x.Equals(@event.Uid)))
                {
                    return Task.FromResult(EventSavingResult.AlreadyAdded);
                }
            }

            _list.Add(@event);
            return Task.FromResult(EventSavingResult.Success);
        }

        public Task<IEnumerable<IEvent>> GetEvents(ulong startVersion, ulong endVersion)
        {
            var re = _list.Where(x => x.Version > startVersion && x.Version <= endVersion);
            return Task.FromResult(re);
        }
    }
}