using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newbe.Claptrap.Autofac
{
    public class AttributeBaseActorTypeRegistrationProvider : IActorTypeRegistrationProvider
    {
        public delegate AttributeBaseActorTypeRegistrationProvider Factory(IEnumerable<Assembly> assemblies);

        public AttributeBaseActorTypeRegistrationProvider(
            IEnumerable<Assembly> assemblies)
        {
            var assembliesArray = assemblies as Assembly[] ?? assemblies.ToArray();

            var impls = assembliesArray
                .SelectMany(x => x.DefinedTypes)
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x => x.GetCustomAttribute<ClaptrapStateInitialFactoryHandlerAttribute>() != null)
                .Select(implType =>
                {
                    var interfaceType = implType.GetInterfaces()
                        .Single(a => a.GetCustomAttribute<ClaptrapStateAttribute>() != null);
                    return new
                    {
                        grainType = implType,
                        igrainType = interfaceType,
                        stateAttr = interfaceType.GetCustomAttribute<ClaptrapStateAttribute>(),
                        eventAttrs = interfaceType.GetCustomAttributes<ClaptrapEventAttribute>(),
                        stateInitialFactoryHandlerAttr =
                            implType.GetCustomAttribute<ClaptrapStateInitialFactoryHandlerAttribute>(),
                        eventHandlerAttrs = implType.GetCustomAttributes<ClaptrapEventHandlerAttribute>(),
                        eventStoreAttr = implType.GetCustomAttribute<EventStoreAttribute>(),
                        stateStoreAttr = implType.GetCustomAttribute<StateStoreAttribute>(),
                    };
                })
                .ToArray();


            var actorTypeRegistrations = impls
                .Select(x => new ActorTypeRegistration
                {
                    ActorTypeCode = GetActorTypeCode(x.stateAttr),
                    ActorStateDataType = x.stateAttr.StateDataType,
                    StateInitialFactoryHandlerType = x.stateInitialFactoryHandlerAttr.StateInitialFactoryHandlerType ??
                                                     typeof(DefaultInitialStateDataFactoryHandler),
                })
                .ToArray();

            var eventHandlerTypeRegistrations = impls
                .SelectMany(x =>
                {
                    return x.eventAttrs
                        .Join(
                            x.eventHandlerAttrs,
                            a => a.EventDataType,
                            a => a.EventDataType,
                            (a, b) => (eventAttr: a, eventHandlerAttr: b))
                        .Select(e => new EventTypeHandlerRegistration
                        {
                            ActorTypeCode = GetActorTypeCode(x.stateAttr),
                            EventTypeCode = GetEventTypeCode(e.eventAttr),
                            EventDataType = e.eventAttr.EventDataType,
                            EventHandlerType = e.eventHandlerAttr.EventHandlerType
                        });
                })
                .ToArray();

            var eventStoreRegistrations = impls
                .Select(x => new EventStoreRegistration
                {
                    ActorTypeCode = GetActorTypeCode(x.stateAttr),
                    EventStoreProvider = x.eventStoreAttr.EventStoreProvider
                })
                .ToArray();

            var stateStoreRegistrations = impls
                .Select(x => new StateStoreRegistration
                {
                    ActorTypeCode = GetActorTypeCode(x.stateAttr),
                    StateStoreProvider = x.stateStoreAttr.StateStoreProvider,
                });
            
            ClaptrapRegistration = new ClaptrapRegistration
            {
                ActorTypeRegistrations = actorTypeRegistrations,
                EventHandlerTypeRegistrations = eventHandlerTypeRegistrations,
                EventStoreRegistrations = eventStoreRegistrations,
                StateStoreRegistrations = stateStoreRegistrations
            };

            static string GetActorTypeCode(ClaptrapStateAttribute attr)
            {
                return attr.ActorTypeCode ??
                       attr.StateDataType.FullName;
            }

            static string GetEventTypeCode(ClaptrapEventAttribute attr)
            {
                return attr.EventTypeCode ??
                       attr.EventDataType.FullName;
            }
        }

        public ClaptrapRegistration ClaptrapRegistration { get; }
    }
}