using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter
{
    public class EmptyMinionProxy : IMinionProxy
    {
        public Task MasterEventReceivedAsync(IEnumerable<IEvent> events)
        {
            return Task.CompletedTask;
        }

        public static EmptyMinionProxy Instance { get; } = new EmptyMinionProxy();
    }
}