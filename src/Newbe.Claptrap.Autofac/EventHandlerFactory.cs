using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.Autofac
{
    public class EventHandlerFactory : IEventHandlerFactory
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger<EventHandlerFactory> _logger;
        private readonly IEventHandlerRegister _eventHandlerRegister;

        public EventHandlerFactory(
            ILifetimeScope lifetimeScope,
            ILogger<EventHandlerFactory> logger,
            IEventHandlerRegister eventHandlerRegister)
        {
            _lifetimeScope = lifetimeScope;
            _logger = logger;
            _eventHandlerRegister = eventHandlerRegister;
        }

        public IEventHandler Create(IEventContext eventContext)
        {
            var eventScope = _lifetimeScope.BeginLifetimeScope();
            var handlerType =
                _eventHandlerRegister.FindHandlerType(
                    eventContext.State.Identity.TypeCode,
                    eventContext.Event.EventType);
            if (handlerType == null)
            {
                _logger.LogError("handlerType not found, event context :@{eventContext}", eventContext);
                throw new EventHandlerNotFoundException(
                    eventContext.State.Identity.TypeCode,
                    eventContext.Event.EventType);
            }

            var handler = (IEventHandler) eventScope.Resolve(handlerType);
            return handler;
        }
    }
}