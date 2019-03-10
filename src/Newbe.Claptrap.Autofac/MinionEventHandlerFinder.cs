using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    internal class MinionEventHandlerFinder : IMinionEventHandlerFinder
    {
        private readonly IActorMetadataProvider _actorMetadataProvider;

        public MinionEventHandlerFinder(
            IActorMetadataProvider actorMetadataProvider)
        {
            _actorMetadataProvider = actorMetadataProvider;
        }

        public IEnumerable<MinionEventHandlerRegistration> FindAll(Type[] allTypes)
        {
            IMinionEventHandlerFinder[] finders =
            {
                new NamespaceFactoryFinder(_actorMetadataProvider)
            };
            var re = finders.SelectMany(x => x.FindAll(allTypes));
            return re;
        }

        private class NamespaceFactoryFinder : IMinionEventHandlerFinder
        {
            private readonly IActorMetadataProvider _actorMetadataProvider;

            public NamespaceFactoryFinder(
                IActorMetadataProvider actorMetadataProvider)
            {
                _actorMetadataProvider = actorMetadataProvider;
            }

            public IEnumerable<MinionEventHandlerRegistration> FindAll(Type[] types)
            {
                var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();

                var grainImpls = types.Where(ReflectionHelper.IsMinionGrainImplement);
                foreach (var grainType in grainImpls)
                {
                    var eventHandlerTypes = types
                        .Where(x => x.Namespace.StartsWith(grainType.Namespace))
                        .Where(x => x.GetInterface(nameof(IEventHandler)) != null);

                    var actorKind = (IMinionKind) ReflectionHelper.GetActorKind(grainType);

                    foreach (var type in eventHandlerTypes)
                    {
                        var baseTypes = ReflectionHelper.GetBaseTypes(type);
                        foreach (var baseType in baseTypes)
                        {
                            if (baseType.IsGenericType
                                && baseType.GetGenericTypeDefinition() == typeof(MinionEventHandlerBase<,>))
                            {
                                // using attribute to check eventType 
                                var minionEventComponentAttribute =
                                    type.GetCustomAttribute<MinionEventComponentAttribute>();
                                var eventType = minionEventComponentAttribute?.EventType;

                                // using EventDataType to check eventType
                                if (string.IsNullOrEmpty(eventType))
                                {
                                    var eventDataType = baseType.GenericTypeArguments[1];
                                    var claptrapMetadata = actorMetadataCollection[actorKind]
                                        .ClaptrapMetadata;
                                    // todo handle exception if there are not single one
                                    eventType = claptrapMetadata.ClaptrapEventMetadata
                                        .Single(x => x.EventDataType == eventDataType).EventType;
                                }

                                var key = new MinionEventHandlerRegistrationKey(actorKind, eventType);
                                var re = new MinionEventHandlerRegistration(key, type);
                                yield return re;
                            }
                        }
                    }
                }
            }
        }
    }
}