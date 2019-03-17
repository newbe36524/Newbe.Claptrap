using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Orleans;

namespace Newbe.Claptrap.Orleans
{
    public interface IMinionGrain : IGrainWithStringKey
    {
        /// <summary>
        /// handle event witch are not handled by other methods
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task HandleOtherEvent(IEvent @event);
    }
}