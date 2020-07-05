using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapMinionGrain : IClaptrapGrain
    {
        Task MasterEventReceivedAsync(IEnumerable<IEvent> events);

        Task WakeAsync();
    }
}