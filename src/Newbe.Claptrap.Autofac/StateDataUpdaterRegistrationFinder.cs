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
    internal class StateDataUpdaterRegistrationFinder : IStateDataUpdaterRegistrationFinder
    {
        private readonly IActorMetadataProvider _actorMetadataProvider;

        public StateDataUpdaterRegistrationFinder(
            IActorMetadataProvider actorMetadataProvider)
        {
            _actorMetadataProvider = actorMetadataProvider;
        }

        public IEnumerable<StateDataUpdaterRegistration> FindAll(Type[] types)
        {
            IStateDataUpdaterRegistrationFinder[] finders =
            {
                new NoneStateFactoryFinder(_actorMetadataProvider),
                new NamespaceFactoryFinder(_actorMetadataProvider),
            };
            var re = finders.SelectMany(x => x.FindAll(types));
            return re;
        }

        private class NoneStateFactoryFinder : IStateDataUpdaterRegistrationFinder
        {
            private readonly IActorMetadataProvider _actorMetadataProvider;

            public NoneStateFactoryFinder(
                IActorMetadataProvider actorMetadataProvider)
            {
                _actorMetadataProvider = actorMetadataProvider;
            }

            public IEnumerable<StateDataUpdaterRegistration> FindAll(Type[] types)
            {
                var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
                foreach (var minionMetadata in actorMetadataCollection.MinionMetadata)
                {
                    if (minionMetadata.StateDataType == typeof(NoneStateData))
                    {
                        foreach (var claptrapEventMetadata in minionMetadata.ClaptrapEventMetadata)
                        {
                            var key = new StateDataUpdaterRegistrationKey(minionMetadata.MinionKind,
                                claptrapEventMetadata.EventType);
                            var re = new StateDataUpdaterRegistration(key, typeof(NoneStateDataStateDataUpdater));
                            yield return re;
                        }
                    }
                }

                foreach (var claptrapMetadata in actorMetadataCollection.ClaptrapMetadata)
                {
                    if (claptrapMetadata.StateDataType == typeof(NoneStateData))
                    {
                        foreach (var claptrapEventMetadata in claptrapMetadata.ClaptrapEventMetadata)
                        {
                            var key = new StateDataUpdaterRegistrationKey(claptrapMetadata.ClaptrapKind,
                                claptrapEventMetadata.EventType);
                            var re = new StateDataUpdaterRegistration(key, typeof(NoneStateDataStateDataUpdater));
                            yield return re;
                        }
                    }
                }
            }
        }

        private class NamespaceFactoryFinder : IStateDataUpdaterRegistrationFinder
        {
            private readonly IActorMetadataProvider _actorMetadataProvider;

            public NamespaceFactoryFinder(
                IActorMetadataProvider actorMetadataProvider)
            {
                _actorMetadataProvider = actorMetadataProvider;
            }

            public IEnumerable<StateDataUpdaterRegistration> FindAll(Type[] types)
            {
                var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();

                var grainImpls = types.Where(ReflectionHelper.IsClaptrapOrMinionGrainImplement);
                foreach (var grainType in grainImpls)
                {
                    var updaterTypeInSubNamespace = types
                        .Where(x => x.Namespace.StartsWith(grainType.Namespace))
                        .Where(x => x.GetInterface(nameof(IStateDataUpdater)) != null);

                    var actorKind = ReflectionHelper.GetActorKind(grainType);

                    foreach (var type in updaterTypeInSubNamespace)
                    {
                        var baseTypes = ReflectionHelper.GetBaseTypes(type);
                        foreach (var baseType in baseTypes)
                        {
                            if (baseType.IsGenericType
                                && baseType.GetGenericTypeDefinition() == typeof(StateDataUpdaterBase<,>))
                            {
                                // using attribute to check eventType 
                                string eventType;
                                switch (actorKind.ActorType)
                                {
                                    case ActorType.Claptrap:
                                        var claptrapEventComponentAttribute =
                                            type.GetCustomAttribute<ClaptrapEventComponentAttribute>();
                                        eventType = claptrapEventComponentAttribute?.EventType;
                                        break;
                                    case ActorType.Minion:
                                        var minionEventComponentAttribute =
                                            type.GetCustomAttribute<MinionEventComponentAttribute>();
                                        eventType = minionEventComponentAttribute?.EventType;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                // using EventDataType to check eventType
                                if (string.IsNullOrEmpty(eventType))
                                {
                                    var eventDataType = baseType.GenericTypeArguments[1];
                                    ClaptrapMetadata claptrapMetadata;
                                    switch (actorKind.ActorType)
                                    {
                                        case ActorType.Claptrap:
                                            claptrapMetadata = actorMetadataCollection[(IClaptrapKind) actorKind];

                                            break;
                                        case ActorType.Minion:
                                            claptrapMetadata = actorMetadataCollection[(IMinionKind) actorKind]
                                                .ClaptrapMetadata;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }

                                    // todo handle exception if there are not single one
                                    eventType = claptrapMetadata.ClaptrapEventMetadata
                                        .Single(x => x.EventDataType == eventDataType).EventType;
                                }

                                var key = new StateDataUpdaterRegistrationKey(actorKind, eventType);
                                var re = new StateDataUpdaterRegistration(key, type);
                                yield return re;
                            }
                        }
                    }
                }
            }
        }
    }
}