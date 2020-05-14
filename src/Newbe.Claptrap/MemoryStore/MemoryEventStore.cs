using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Newbe.Claptrap.MemoryStore
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

        public Task SaveEventAsync(IEvent @event)
        {
            _list.Add(@event);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            var re = _list.Where(x => x.Version > startVersion && x.Version <= endVersion).OrderBy(x => x.Version);
            return Task.FromResult<IEnumerable<IEvent>>(re);
        }
    }
}