using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public class MemoryEventStore : IEventStore
    {
        public delegate MemoryEventStore Factory(IActorIdentity identity);

        private readonly IList<IEvent> _list;

        public MemoryEventStore(IActorIdentity identity)
        {
            Identity = identity;
            _list = new List<IEvent>();
        }

        public IActorIdentity Identity { get; }

        public Task<EventSavingResult> SaveEvent(IEvent @event)
        {
            if (_list.Any(x =>
                x.ActorIdentity.TypeCode == @event.ActorIdentity.TypeCode
                && x.EventTypeCode == @event.EventTypeCode
                && x.Uid == @event.Uid))
            {
                return Task.FromResult(EventSavingResult.AlreadyAdded);
            }

            _list.Add(@event);
            return Task.FromResult(EventSavingResult.Success);
        }

        public Task<IEnumerable<IEvent>> GetEvents(long startVersion, long endVersion)
        {
            var re = _list.Where(x => x.Version > startVersion && x.Version <= endVersion).OrderBy(x => x.Version);
            return Task.FromResult<IEnumerable<IEvent>>(re);
        }
    }
}