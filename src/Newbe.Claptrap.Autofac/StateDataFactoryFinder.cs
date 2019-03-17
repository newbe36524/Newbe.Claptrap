using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.Orleans;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public class StateDataFactoryFinder : IStateDataFactoryFinder
    {
        private readonly IActorMetadataProvider _actorMetadataProvider;

        public StateDataFactoryFinder(
            IActorMetadataProvider actorMetadataProvider)
        {
            _actorMetadataProvider = actorMetadataProvider;
        }

        public IEnumerable<DefaultStateDataFactoryRegistration> FindAll(Type[] types)
        {
            IStateDataFactoryFinder[] finders =
            {
                new NoneStateDataFinder(_actorMetadataProvider),
                new NamespaceFactoryFinder()
            };

            var re = finders.SelectMany(x => x.FindAll(types));
            return re;
        }


        private class NoneStateDataFinder : IStateDataFactoryFinder
        {
            private readonly IActorMetadataProvider _actorMetadataProvider;

            public NoneStateDataFinder(
                IActorMetadataProvider actorMetadataProvider)
            {
                _actorMetadataProvider = actorMetadataProvider;
            }

            public IEnumerable<DefaultStateDataFactoryRegistration> FindAll(Type[] types)
            {
                var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
                foreach (var claptrapMetadata in actorMetadataCollection.ClaptrapMetadata)
                {
                    if (claptrapMetadata.StateDataType == typeof(NoneStateData))
                    {
                        var key = new DefaultStateDataFactoryRegistrationKey(claptrapMetadata.ClaptrapKind);
                        yield return new DefaultStateDataFactoryRegistration(
                            typeof(NoneStateDataStateDataFactory), key);
                    }
                }

                foreach (var minionMetadata in actorMetadataCollection.MinionMetadata)
                {
                    if (minionMetadata.StateDataType == typeof(NoneStateData))
                    {
                        var key = new DefaultStateDataFactoryRegistrationKey(minionMetadata.MinionKind);
                        yield return new DefaultStateDataFactoryRegistration(
                            typeof(NoneStateDataStateDataFactory), key);
                    }
                }
            }
        }

        private class NamespaceFactoryFinder : IStateDataFactoryFinder
        {
            public IEnumerable<DefaultStateDataFactoryRegistration> FindAll(Type[] types)
            {
                var grainImpls = types.Where(ReflectionHelper.IsClaptrapOrMinionGrainImplement);
                foreach (var grainType in grainImpls)
                {
                    var factorTypesInSubNamespace = types
                        .Where(x => x.Namespace.StartsWith(grainType.Namespace))
                        .Where(x => x.GetInterface(nameof(IStateDataFactory)) != null);

                    var actorKind = ReflectionHelper.GetActorKind(grainType);

                    foreach (var type in factorTypesInSubNamespace)
                    {
                        var baseTypes = ReflectionHelper.GetBaseTypes(type);
                        foreach (var baseType in baseTypes)
                        {
                            if (baseType.IsGenericType
                                && baseType.GetGenericTypeDefinition() == typeof(StateDataFactory<>))
                            {
                                var stateDataType = baseType.GenericTypeArguments[0];
                                if (stateDataType != typeof(NoneStateDataStateDataUpdater))
                                {
                                    var key = new DefaultStateDataFactoryRegistrationKey(actorKind);
                                    yield return new DefaultStateDataFactoryRegistration(type, key);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}