using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventStore.Memory
{
    /// <inheritdoc />
    /// <summary>
    /// Memory event store for test.Do not use this for production.
    /// </summary>
    public class MemoryEventStore : IEventStore
    {
        private readonly IList<IEvent> _list = new List<IEvent>();
        private readonly IDictionary<IEventUid, IEvent> _dictionary = new Dictionary<IEventUid, IEvent>();

        public MemoryEventStore(IActorIdentity identity)
        {
            Identity = identity;
        }

        public IActorIdentity Identity { get; }

        public Task<EventSavingResult> SaveEvent(IEvent @event)
        {
            if (@event.Uid != null)
            {
                if (_dictionary.ContainsKey(@event.Uid))
                {
                    return Task.FromResult(EventSavingResult.AlreadyAdded);
                }

                _dictionary.Add(@event.Uid, @event);
            }
            else
            {
                _list.Add(@event);
            }

            return Task.FromResult(EventSavingResult.Success);
        }

        public Task<IEnumerable<IEvent>> GetEvents(ulong startVersion, ulong endVersion)
        {
            var re = Events
                .Where(x => x.Version > startVersion && x.Version <= endVersion)
                .OrderBy(x => x.Version)
                .AsEnumerable();
            return Task.FromResult(re);
        }

        private IEnumerable<IEvent> Events
        {
            get
            {
                foreach (var @event in _dictionary.Values)
                {
                    yield return @event;
                }

                foreach (var @event in _list)
                {
                    yield return @event;
                }
            }
        }
    }
}