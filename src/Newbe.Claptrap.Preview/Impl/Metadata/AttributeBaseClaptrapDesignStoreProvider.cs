using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Attributes;
using Newbe.Claptrap.Preview.Orleans;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public class AttributeBaseClaptrapDesignStoreProvider : IClaptrapDesignStoreProvider
    {
        public delegate AttributeBaseClaptrapDesignStoreProvider Factory(IEnumerable<Type> types);

        private readonly IEnumerable<Type> _types;
        private readonly ClaptrapDesignStore.Factory _claptrapDesignStoreFactory;

        public AttributeBaseClaptrapDesignStoreProvider(
            IEnumerable<Type> types,
            ClaptrapDesignStore.Factory claptrapDesignStoreFactory)
        {
            _types = types;
            _claptrapDesignStoreFactory = claptrapDesignStoreFactory;
        }

        public IClaptrapDesignStore Create()
        {
            var types = _types as Type[] ?? _types.ToArray();

            var impls = types
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

            var re = _claptrapDesignStoreFactory();

            var claptrapDesigns = impls
                .Select(x =>
                {
                    var claptrapDesign = new ClaptrapDesign
                    {
                        Identity = new ClaptrapIdentity(string.Empty, GetActorTypeCode(x.stateAttr)),
                        ActorStateDataType = x.stateAttr.StateDataType,
                        InitialStateDataFactoryType = x.stateInitialFactoryHandlerAttr.StateInitialFactoryHandlerType
                                                      ?? typeof(DefaultInitialStateDataFactory),
                        StateHolderFactoryType = typeof(DeepClonerStateHolderFactory),
                        EventLoaderFactoryType = x.eventStoreAttr.EventLoaderFactoryType,
                        EventSaverFactoryType = x.eventStoreAttr.EventSaverFactoryType,
                        StateLoaderFactoryType = x.stateStoreAttr.StateLoaderFactoryType,
                        StateSaverFactoryType = x.stateStoreAttr.StateSaverFactoryType,
                        EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory),
                        StateSavingOptions = new StateSavingOptions
                        {
                            SavingWindowTime = TimeSpan.FromSeconds(10),
                            SaveWhenDeactivateAsync = true,
                            SavingWindowVersionLimit = 1000
                        },
                    };
                    var handlerDesigns = x.eventAttrs.Select(e => new ClaptrapEventHandlerDesign
                    {
                        EventTypeCode = GetEventTypeCode(e),
                        EventDataType = e.EventDataType,
                        EventHandlerType = x.eventHandlerAttrs.Single(a => a.EventDataType == e.EventDataType)
                            .EventHandlerType
                    }).ToArray();
                    claptrapDesign.EventHandlerDesigns =
                        new Dictionary<string, IClaptrapEventHandlerDesign>(
                            handlerDesigns.ToDictionary(a => a.EventTypeCode, a => (IClaptrapEventHandlerDesign) a));
                    return claptrapDesign;
                })
                .ToArray();

            foreach (var design in claptrapDesigns)
            {
                re.AddOrReplace(design);
            }

            return re;

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
    }
}