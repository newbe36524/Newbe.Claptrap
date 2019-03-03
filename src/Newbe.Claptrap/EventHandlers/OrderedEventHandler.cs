using System.Collections.Generic;
using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.EventHandlers
{
    public class OrderedEventHandler : IEventHandler
    {
        private readonly IEnumerable<IEventHandler> _eventHandlers;

        public OrderedEventHandler(
            IEnumerable<IEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        public async Task HandleEvent(IEventContext eventContext)
        {
            foreach (var handler in _eventHandlers)
            {
                await handler.HandleEvent(eventContext);
            }
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var eventHandler in _eventHandlers)
            {
                await eventHandler.DisposeAsync();
            }
        }
    }
}