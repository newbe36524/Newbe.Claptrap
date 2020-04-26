using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    public class ClaptrapCustomerModule : Module
    {
        private readonly ILogger<ClaptrapCustomerModule> _logger;
        private readonly ClaptrapRegistration _claptrapRegistration;

        public ClaptrapCustomerModule(
            ILogger<ClaptrapCustomerModule> logger,
            ClaptrapRegistration claptrapRegistration)
        {
            _logger = logger;
            _claptrapRegistration = claptrapRegistration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(_claptrapRegistration)
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<ClaptrapRegistrationAccessor>()
                .As<IClaptrapRegistrationAccessor>()
                .SingleInstance();
            
            _logger.LogDebug("start to register actor types ");
            foreach (var actorTypeRegistration in _claptrapRegistration.ActorTypeRegistrations)
            {
                var actorTypeCode = actorTypeRegistration.ActorTypeCode;
                _logger.LogDebug("start to register actor type : {actorTypeCode}", actorTypeCode);
                RegisterActorType(actorTypeRegistration);
                _logger.LogDebug("actor type registration for '{actorTypeCode}' done", actorTypeCode);
            }

            foreach (var eventHandlerTypeRegistration in _claptrapRegistration.EventHandlerTypeRegistrations)
            {
                var actorTypeCode = eventHandlerTypeRegistration.ActorTypeCode;
                var eventTypeCode = eventHandlerTypeRegistration.EventTypeCode;
                _logger.LogDebug("start to register '{eventTypeCode}' event handler for '{actorTypeCode}'",
                    eventTypeCode,
                    actorTypeCode);
                RegisterEventHandlerType(eventHandlerTypeRegistration);
                _logger.LogDebug("event handler registration for '{eventTypeCode}' with '{actorTypeCode}'",
                    eventTypeCode,
                    actorTypeCode);
            }

            _logger.LogDebug("actor type registration done");

            _logger.LogInformation("{count} actorType have been registered into container",
                _claptrapRegistration.ActorTypeRegistrations.Count());

            void RegisterActorType(ActorTypeRegistration actorTypeRegistration)
            {
                builder.RegisterType(actorTypeRegistration.StateInitialFactoryHandlerType)
                    .Keyed<IInitialStateDataFactoryHandler>(actorTypeRegistration.ActorTypeCode)
                    .InstancePerLifetimeScope();
            }

            void RegisterEventHandlerType(EventTypeHandlerRegistration eventHandlerTypeRegistration)
            {
                builder.RegisterType(eventHandlerTypeRegistration.EventHandlerType)
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }
        }
    }
}