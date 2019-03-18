using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.EventHandlers
{
    public class ClaptrapEventPublishEventHandler : IEventHandler
    {
        private readonly IEnumerable<IEventPublishChannel> _eventPublishChannels;

        public ClaptrapEventPublishEventHandler(IEnumerable<IEventPublishChannel> eventPublishChannels)
        {
            _eventPublishChannels = eventPublishChannels;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var eventPublishChannel in _eventPublishChannels)
            {
                await eventPublishChannel.DisposeAsync();
            }
        }

        public Task HandleEvent(IEventContext eventContext)
        {
            return Task.WhenAll(_eventPublishChannels.Select(x => x.Publish(eventContext.Event)));
        }
    }
}