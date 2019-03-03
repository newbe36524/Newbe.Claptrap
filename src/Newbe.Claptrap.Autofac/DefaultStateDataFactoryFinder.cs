using System;
using System.Collections.Generic;
using System.Linq;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public class DefaultStateDataFactoryFinder : IDefaultStateDataFactoryFinder
    {
        private readonly IActorMetadataProvider _actorMetadataProvider;

        public DefaultStateDataFactoryFinder(
            IActorMetadataProvider actorMetadataProvider)
        {
            _actorMetadataProvider = actorMetadataProvider;
        }

        public IEnumerable<DefaultStateDataFactoryRegistration> FindAll(Type[] types)
        {
            var factoryTypes = types
                .Where(x => x.GetInterface(nameof(IDefaultStateDataFactory)) != null)
                .ToArray();

            IRegistrationResolver[] resolvers =
            {
                new BaseTypeRegistrationResolver(_actorMetadataProvider)
            };

            var re = factoryTypes.Select(Resolve).Where(x => x != null).ToArray();
            return re;

            DefaultStateDataFactoryRegistration Resolve(Type type)
            {
                foreach (var resolver in resolvers)
                {
                    var registration = resolver.Resolve(type);
                    if (registration != null)
                    {
                        return registration;
                    }
                }

                return null;
            }
        }

        public interface IRegistrationResolver
        {
            DefaultStateDataFactoryRegistration Resolve(Type type);
        }

        /// <summary>
        /// if it is implement of DefaultStateDataFactory&lt;TStateData&gt;, then we thick it is the IDefaultStateDataFactory for the actor which has the same StateDataType in actor metadata.
        /// </summary>
        public class BaseTypeRegistrationResolver : IRegistrationResolver
        {
            private readonly IActorMetadataProvider _actorMetadataProvider;

            public BaseTypeRegistrationResolver(
                IActorMetadataProvider actorMetadataProvider)
            {
                _actorMetadataProvider = actorMetadataProvider;
            }

            public DefaultStateDataFactoryRegistration Resolve(Type type)
            {
                var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
                var baseTypes = ReflectionHelper.GetBaseType(type);
                foreach (var baseType in baseTypes)
                {
                    if (baseType.IsGenericType
                        && baseType.GetGenericTypeDefinition() == typeof(DefaultStateDataFactory<>))
                    {
                        var stateDataType = baseType.GenericTypeArguments[0];
                        foreach (var metadata in actorMetadataCollection.ClaptrapMetadata)
                        {
                            if (metadata.StateDataType == stateDataType)
                            {
                                var key = new DefaultStateDataFactoryRegistrationKey(metadata.ClaptrapKind);
                                return new DefaultStateDataFactoryRegistration(type, key);
                            }
                        }
                    }
                }

                return null;
            }
        }
    }
}