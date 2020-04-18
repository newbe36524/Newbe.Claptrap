using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap
{
    public class Actor : IActor
    {
        private readonly IActorContext _actorContext;
        private readonly IEventHandlerFactory _eventHandlerFactory;

        public Actor(
            IActorContext actorContext,
            IEventHandlerFactory eventHandlerFactory)
        {
            _actorContext = actorContext;
            _eventHandlerFactory = eventHandlerFactory;
        }

        public IState State => _actorContext.State;

        public Task ActivateAsync()
        {
            return _actorContext.InitializeAsync();
        }

        public Task DeactivateAsync()
        {
            return _actorContext.DisposeAsync();
        }

        public async Task HandleEvent(IEvent @event)
        {
            var eventContext = new EventContext(@event, _actorContext);
            IEventHandler handler = null;
            try
            {
                handler = _eventHandlerFactory.Create(eventContext);
                await handler.HandleEvent(eventContext);
            }
            catch (Exception e)
            {
                // TODO log error
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (handler != null)
                {
                    await handler.DisposeAsync();
                }
            }
        }
    }
}