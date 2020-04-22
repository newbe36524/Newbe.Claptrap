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
        private readonly IEnumerable<ActorTypeRegistration> _actorTypeRegistrations;
        private readonly IEnumerable<EventHandlerTypeRegistration> _eventHandlerTypeRegistrations;

        public ClaptrapCustomerModule(
            ILogger<ClaptrapCustomerModule> logger,
            IEnumerable<ActorTypeRegistration> actorTypeRegistrations,
            IEnumerable<EventHandlerTypeRegistration> eventHandlerTypeRegistrations)
        {
            _logger = logger;
            _actorTypeRegistrations = actorTypeRegistrations;
            _eventHandlerTypeRegistrations = eventHandlerTypeRegistrations;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _logger.LogDebug("start to register actor types ");
            foreach (var actorTypeRegistration in _actorTypeRegistrations)
            {
                var actorTypeCode = actorTypeRegistration.ActorTypeCode;
                _logger.LogDebug("start to register actor type : {actorTypeCode}", actorTypeCode);
                RegisterActorType(actorTypeRegistration);
                _logger.LogDebug("actor type registration for '{actorTypeCode}' done", actorTypeCode);
            }

            foreach (var eventHandlerTypeRegistration in _eventHandlerTypeRegistrations)
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
                _actorTypeRegistrations.Count());

            void RegisterActorType(ActorTypeRegistration actorTypeRegistration)
            {
                builder.RegisterType(actorTypeRegistration.StateInitialFactoryHandlerType)
                    .Keyed<IInitialStateDataFactoryHandler>(actorTypeRegistration.ActorTypeCode)
                    .InstancePerLifetimeScope();
                builder.RegisterBuildCallback(scope =>
                {
                    var register = scope.Resolve<IStateDataTypeRegister>();
                    register.RegisterStateDataType(actorTypeRegistration.ActorTypeCode,
                        actorTypeRegistration.ActorStateDataType);
                });
            }

            void RegisterEventHandlerType(EventHandlerTypeRegistration eventHandlerTypeRegistration)
            {
                builder.RegisterType(eventHandlerTypeRegistration.EventHandlerType)
                    .AsSelf()
                    .InstancePerLifetimeScope();
                builder.RegisterBuildCallback(scope =>
                {
                    var register = scope.Resolve<IEventHandlerRegister>();
                    register.RegisterHandler(eventHandlerTypeRegistration.ActorTypeCode,
                        eventHandlerTypeRegistration.EventTypeCode,
                        eventHandlerTypeRegistration.EventHandlerType);
                });
            }
        }
    }
}