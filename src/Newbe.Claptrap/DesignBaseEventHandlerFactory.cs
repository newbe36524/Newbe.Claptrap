using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Design;

namespace Newbe.Claptrap
{
    public class DesignBaseEventHandlerFactory : IEventHandlerFactory
    {
        public delegate DesignBaseEventHandlerFactory Factory(IClaptrapIdentity identity);

        private readonly ILifetimeScope _lifetimeScope;
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly ILogger<DesignBaseEventHandlerFactory> _logger;

        public DesignBaseEventHandlerFactory(
            IClaptrapIdentity identity,
            ILifetimeScope lifetimeScope,
            IClaptrapDesignStore claptrapDesignStore,
            ILogger<DesignBaseEventHandlerFactory> logger)
        {
            _lifetimeScope = lifetimeScope;
            _claptrapDesignStore = claptrapDesignStore;
            _logger = logger;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public IEventHandler Create(IEventContext eventContext)
        {
            try
            {
                return CreateCore();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to create handler for {@context}", eventContext);
                throw;
            }

            IEventHandler CreateCore()
            {
                var eventScope = _lifetimeScope.BeginLifetimeScope();
                var claptrapDesign = _claptrapDesignStore.FindDesign(eventContext.State.Identity);
                var eventEventTypeCode = eventContext.Event.EventTypeCode;
                if (!claptrapDesign.EventHandlerDesigns.TryGetValue(eventEventTypeCode,
                    out var handlerDesign))
                {
                    throw new EventHandlerNotFoundException(eventContext.State.Identity.TypeCode, eventEventTypeCode);
                }

                var handler = (IEventHandler) eventScope.Resolve(handlerDesign.EventHandlerType);
                return handler;
            }
        }
    }
}