using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventChannels
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