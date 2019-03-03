using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventChannels
{
    public interface IEventPublishChannel
    {
        /// <summary>
        /// publish event to minion
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task Publish(IEvent @event);
    }
}