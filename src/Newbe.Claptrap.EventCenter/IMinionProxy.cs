using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter
{
    public interface IMinionProxy
    {
        Task MasterEventReceivedAsync(IEnumerable<IEvent> events);
    }
}