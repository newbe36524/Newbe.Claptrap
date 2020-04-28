using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Context;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventHandler;
using Newbe.Claptrap.Preview.Metadata;

namespace Newbe.Claptrap.Preview
{
    public class EventHandlerFactory : IEventHandlerFactory
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger<EventHandlerFactory> _logger;
        private readonly IClaptrapRegistrationAccessor _claptrapRegistrationAccessor;

        public EventHandlerFactory(
            ILifetimeScope lifetimeScope,
            ILogger<EventHandlerFactory> logger,
            IClaptrapRegistrationAccessor claptrapRegistrationAccessor)
        {
            _lifetimeScope = lifetimeScope;
            _logger = logger;
            _claptrapRegistrationAccessor = claptrapRegistrationAccessor;
        }

        public IEventHandler Create(IEventContext eventContext)
        {
            var eventScope = _lifetimeScope.BeginLifetimeScope();
            var handlerType =
                _claptrapRegistrationAccessor.FindEventHandlerType(
                    eventContext.State.Identity.TypeCode,
                    eventContext.Event.EventTypeCode);
            if (handlerType == null)
            {
                _logger.LogError("handlerType not found, event context :@{eventContext}", eventContext);
                throw new EventHandlerNotFoundException(
                    eventContext.State.Identity.TypeCode,
                    eventContext.Event.EventTypeCode);
            }

            var handler = (IEventHandler) eventScope.Resolve(handlerType);
            return handler;
        }
    }
}