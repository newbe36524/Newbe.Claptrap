using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Preview.Abstractions.Design;
using Newbe.Claptrap.Preview.Attributes;

namespace Newbe.Claptrap.Preview.Impl.Design
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
                        boxImplType = implType,
                        boxInterfaceType = interfaceType,
                        minionAttr = interfaceType.GetCustomAttribute<ClaptrapMinionAttribute>(),
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

            var claptrapDesignTuples = impls
                .Select(x =>
                {
                    var claptrapDesign = new ClaptrapDesign
                    {
                        Identity = new ClaptrapIdentity(string.Empty, GetActorTypeCode(x.stateAttr)),
                        StateDataType = x.stateAttr.StateDataType,
                        InitialStateDataFactoryType = x.stateInitialFactoryHandlerAttr.StateInitialFactoryHandlerType!,
                        StateHolderFactoryType = typeof(DeepClonerStateHolderFactory),
                        EventLoaderFactoryType = x.eventStoreAttr?.EventLoaderFactoryType!,
                        EventSaverFactoryType = x.eventStoreAttr?.EventSaverFactoryType!,
                        StateLoaderFactoryType = x.stateStoreAttr?.StateLoaderFactoryType!,
                        StateSaverFactoryType = x.stateStoreAttr?.StateSaverFactoryType!,
                        ClaptrapBoxInterfaceType = x.boxInterfaceType,
                        ClaptrapBoxImplementationType = x.boxImplType,
                        // TODO EventHandlerFactoryFactoryType 
                        // TODO state Options
                    };
                    var handlerDesigns = x.eventAttrs.Select(e => new ClaptrapEventHandlerDesign
                    {
                        EventTypeCode = GetEventTypeCode(e),
                        EventDataType = e.EventDataType,
                        EventHandlerType = x.eventHandlerAttrs.Single(a => a.EventTypeCode == e.EventTypeCode)
                            .EventHandlerType
                    }).ToArray();
                    claptrapDesign.EventHandlerDesigns =
                        new Dictionary<string, IClaptrapEventHandlerDesign>(
                            handlerDesigns.ToDictionary(a => a.EventTypeCode, a => (IClaptrapEventHandlerDesign) a));
                    return (claptrapDesign, x);
                })
                .ToArray();

            // try to map master and minions
            var typeCodeDic = claptrapDesignTuples
                .ToDictionary(x => x.claptrapDesign.Identity.TypeCode);

            foreach (var (claptrapDesign, x) in claptrapDesignTuples)
            {
                if (x.minionAttr != null)
                {
                    if (typeCodeDic.TryGetValue(x.minionAttr.MasterTypeCode, out var masterDesign))
                    {
                        claptrapDesign.ClaptrapMasterDesign = masterDesign.claptrapDesign;
                    }
                }
            }

            foreach (var claptrapDesign in claptrapDesignTuples.Select(x => x.claptrapDesign))
            {
                re.AddOrReplace(claptrapDesign);
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