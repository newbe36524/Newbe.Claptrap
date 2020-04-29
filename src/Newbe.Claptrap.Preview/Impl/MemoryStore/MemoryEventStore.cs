using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl.MemoryStore
{
    [ExcludeFromCodeCoverage]
    public class MemoryEventStore : IEventLoader, IEventSaver
    {
        public delegate MemoryEventStore Factory(IClaptrapIdentity identity);

        private readonly IList<IEvent> _list;

        public MemoryEventStore(IClaptrapIdentity identity)
        {
            Identity = identity;
            _list = new List<IEvent>();
        }

        public IClaptrapIdentity Identity { get; }

        public Task<EventSavingResult> SaveEventAsync(IEvent @event)
        {
            if (_list.Any(x =>
                x.ClaptrapIdentity.TypeCode == @event.ClaptrapIdentity.TypeCode
                && x.EventTypeCode == @event.EventTypeCode
                && x.Uid == @event.Uid))
            {
                return Task.FromResult(EventSavingResult.AlreadyAdded);
            }

            _list.Add(@event);
            return Task.FromResult(EventSavingResult.Success);
        }

        public Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            var re = _list.Where(x => x.Version > startVersion && x.Version <= endVersion).OrderBy(x => x.Version);
            return Task.FromResult<IEnumerable<IEvent>>(re);
        }
    }
}