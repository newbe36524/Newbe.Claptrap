using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;

namespace Newbe.Claptrap.Dapr.Core
{
    public interface IClaptrapMinionActor : IActor
    {
        Task MasterEventReceivedAsync(IEnumerable<IEvent> events);

        Task MasterEventReceivedJsonAsync(IEnumerable<EventJsonModel> events);

        Task WakeAsync();
    }
}