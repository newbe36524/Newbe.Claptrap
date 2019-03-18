using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.EventChannels
{
    public interface IEventPublishChannel : IAsyncDisposable
    {
        /// <summary>
        /// publish event to minion
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task Publish(IEvent @event);
    }
}