using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;

namespace Newbe.Claptrap.Dapr.Core
{
    public interface IClaptrapMinionActor : IActor
    {
        IEventSerializer<EventJsonModel> EventSerializer { get; }
        IClaptrap Claptrap { get; }

        async Task MasterEventReceivedAsync(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                await Claptrap.HandleEventAsync(@event);
            }
        }

        async Task MasterEventReceivedJsonAsync(IEnumerable<EventJsonModel> events)
        {
            var items = events.Select(EventSerializer.Deserialize);
            foreach (var @event in items)
            {
                await Claptrap.HandleEventAsync(@event);
            }
        }

        Task WakeAsync()
        {
            return Task.CompletedTask;
        }
    }
}