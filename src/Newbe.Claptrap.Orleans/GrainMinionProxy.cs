using System.Collections.Generic;
using System.Threading.Tasks;
using Newbe.Claptrap.EventCenter;

namespace Newbe.Claptrap.Orleans
{
    public class GrainMinionProxy : IMinionProxy
    {
        public delegate GrainMinionProxy Factory(IClaptrapMinionGrain claptrapMinionGrain);

        private readonly IClaptrapMinionGrain _claptrapMinionGrain;

        public GrainMinionProxy(
            IClaptrapMinionGrain claptrapMinionGrain)
        {
            _claptrapMinionGrain = claptrapMinionGrain;
        }

        public Task MasterEventReceivedAsync(IEnumerable<IEvent> events)
        {
            return _claptrapMinionGrain.MasterEventReceivedAsync(events);
        }
    }
}