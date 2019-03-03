using System;
using System.Collections.Generic;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    class StateDataUpdaterRegistrationFinder : IStateDataUpdaterRegistrationFinder
    {
        private readonly IActorMetadataProvider _actorMetadataProvider;

        public StateDataUpdaterRegistrationFinder(
            IActorMetadataProvider actorMetadataProvider)
        {
            _actorMetadataProvider = actorMetadataProvider;
        }

        public IEnumerable<StateDataUpdaterRegistration> FindAll(Type[] types)
        {
            IRegistrationResolver[] finders =
            {
                new BaseTypeRegistrationResolver(_actorMetadataProvider),
            };
            var re = FindAllCore();
            return re;

            IEnumerable<StateDataUpdaterRegistration> FindAllCore()
            {
                foreach (var type in types)
                {
                    foreach (var finder in finders)
                    {
                        var registration = finder.Resolve(type);
                        if (registration != null)
                        {
                            yield return (StateDataUpdaterRegistration) registration;
                        }
                    }
                }
            }
        }

        public interface IRegistrationResolver
        {
            StateDataUpdaterRegistration Resolve(Type type);
        }

        public class BaseTypeRegistrationResolver : IRegistrationResolver
        {
            private readonly IActorMetadataProvider _actorMetadataProvider;

            public BaseTypeRegistrationResolver(
                IActorMetadataProvider actorMetadataProvider)
            {
                _actorMetadataProvider = actorMetadataProvider;
            }

            public StateDataUpdaterRegistration Resolve(Type type)
            {
                var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
                var baseTypes = ReflectionHelper.GetBaseType(type);
                foreach (var baseType in baseTypes)
                {
                    if (baseType.IsGenericType
                        && baseType.GetGenericTypeDefinition() == typeof(StateDataUpdaterBase<,>))
                    {
                        var stateDataType = baseType.GenericTypeArguments[0];
                        var eventDataType = baseType.GenericTypeArguments[1];
                        foreach (var metadata in actorMetadataCollection.ClaptrapMetadata)
                        {
                            if (metadata.StateDataType == stateDataType)
                            {
                                foreach (var actorEventMetadata in metadata.ClaptrapEventMetadata)
                                {
                                    if (actorEventMetadata.EventDataType == eventDataType)
                                    {
                                        var key = new StateDataUpdaterRegistrationKey(metadata.ClaptrapKind,
                                            actorEventMetadata.EventType);
                                        var re = new StateDataUpdaterRegistration(key, type);
                                        return re;
                                    }
                                }
                            }
                        }
                    }
                }

                return null;
            }
        }
    }
}