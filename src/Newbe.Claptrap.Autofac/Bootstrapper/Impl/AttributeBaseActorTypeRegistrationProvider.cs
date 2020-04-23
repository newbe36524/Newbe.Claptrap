using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Autofac
{
    public class AttributeBaseActorTypeRegistrationProvider : IActorTypeRegistrationProvider
    {
        public delegate AttributeBaseActorTypeRegistrationProvider Factory(IEnumerable<Assembly> assemblies);

        public AttributeBaseActorTypeRegistrationProvider(
            IEnumerable<Assembly> assemblies)
        {
            var assembliesArray = assemblies as Assembly[] ?? assemblies.ToArray();
            var actorTypeRegistrations =
                FindActors(assembliesArray)
                    .ToArray();
            var eventHandlerTypeRegistrations =
                FindEventHandlers(assembliesArray, actorTypeRegistrations)
                    .ToArray();

            ClaptrapRegistration = new ClaptrapRegistration
            {
                ActorTypeRegistrations = actorTypeRegistrations,
                EventHandlerTypeRegistrations = eventHandlerTypeRegistrations
            };
        }

        public ClaptrapRegistration ClaptrapRegistration { get; }

        private static IEnumerable<ActorTypeRegistration> FindActors(IEnumerable<Assembly> assemblies)
        {
            var allClass = assemblies.SelectMany(x => x.ExportedTypes)
                .Where(x => x.IsClass && !x.IsAbstract)
                .ToArray();

            var claptrapGrains = allClass
                .Where(x => x.GetInterface(typeof(IClaptrapGrain).FullName) != null)
                .ToArray();
            var actorTypeRegistrations = claptrapGrains
                .Select(x =>
                {
                    var actorStateDataAttribute = x.GetCustomAttribute<ClaptrapStateAttribute>();
                    Debug.Assert(actorStateDataAttribute != null, nameof(actorStateDataAttribute) + " != null");
                    return new ActorTypeRegistration
                    {
                        ActorTypeCode = actorStateDataAttribute.ActorTypeCode ??
                                        actorStateDataAttribute.StateDataType.FullName,
                        ActorStateDataType = actorStateDataAttribute.StateDataType,
                        StateInitialFactoryHandlerType = actorStateDataAttribute?.InitialStateDataFactoryHandlerType ??
                                                         typeof(DefaultInitialStateDataFactoryHandler),
                    };
                })
                .ToArray();
            return actorTypeRegistrations;
        }

        private IEnumerable<EventHandlerTypeRegistration> FindEventHandlers(
            IEnumerable<Assembly> assemblies, IEnumerable<ActorTypeRegistration> actorTypeRegistrations)
        {
            var allClass = assemblies.SelectMany(x => x.ExportedTypes)
                .Where(x => x.IsClass && !x.IsAbstract)
                .ToArray();
            var eventHandlerTypes = allClass
                .Where(x => x.GetInterface(typeof(IEventHandler).FullName) != null)
                .ToArray();
            var actorDic = actorTypeRegistrations.ToDictionary(x => x.ActorStateDataType);
            var eventHandlerTypeRegistrations = eventHandlerTypes
                .Select(x =>
                {
                    var eventHandlerAttribute = x.GetCustomAttribute<ClaptrapEventHandlerAttribute>();
                    Debug.Assert(eventHandlerAttribute != null, nameof(eventHandlerAttribute) + " != null");
                    if (!actorDic.TryGetValue(eventHandlerAttribute.ActorStateDataType, out var actorTypeRegistration))
                    {
                        // TODO missing Type
                        throw new Exception("missing actor state type");
                    }

                    var eventHandlerTypeRegistration = new EventHandlerTypeRegistration
                    {
                        EventHandlerType = x,
                        EventTypeCode = eventHandlerAttribute.EventTypeCode ??
                                        eventHandlerAttribute.EventDateType.FullName,
                        ActorTypeCode = actorTypeRegistration.ActorTypeCode
                    };
                    return eventHandlerTypeRegistration;
                })
                .ToArray();
            return eventHandlerTypeRegistrations;
        }
    }
}