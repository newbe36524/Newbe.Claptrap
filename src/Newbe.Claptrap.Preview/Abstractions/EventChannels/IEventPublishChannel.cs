using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventChannels
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