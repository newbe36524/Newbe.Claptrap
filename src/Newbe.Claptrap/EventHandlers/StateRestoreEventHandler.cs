using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.EventHandlers
{
    /// <summary>
    /// if inner handler throw a exception when handle event, this handler will try to restore actor state
    /// </summary>
    public class StateRestoreEventHandler :
        IEventHandler
    {
        private readonly IActorContext _actorContext;
        private readonly IEventHandler _eventHandler;

        public StateRestoreEventHandler(
            IEventHandler eventHandler,
            IActorContext actorContext)
        {
            _actorContext = actorContext;
            _eventHandler = eventHandler;
        }

        public ValueTask DisposeAsync()
        {
            return _eventHandler.DisposeAsync();
        }

        public async Task HandleEvent(IEventContext eventContext)
        {
            try
            {
                await _eventHandler.HandleEvent(eventContext);
            }
            catch (Exception e)
            {
                // todo log
                await _actorContext.InitializeAsync();
                throw;
            }
        }
    }
}