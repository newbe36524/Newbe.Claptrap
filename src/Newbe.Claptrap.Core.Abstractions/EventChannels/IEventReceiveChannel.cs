using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventChannels
{
    public interface IEventReceiveChannel
    {
        /// <summary>
        /// receive event from claptrap
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task Receive(IEvent @event);
    }
}