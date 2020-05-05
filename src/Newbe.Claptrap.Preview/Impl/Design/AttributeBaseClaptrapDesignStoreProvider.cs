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

            var metadata = types
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
                        eventAttrs = interfaceType.GetCustomAttributes<ClaptrapEventAttribute>().ToArray(),
                        stateInitialFactoryHandlerAttr =
                            implType.GetCustomAttribute<ClaptrapStateInitialFactoryHandlerAttribute>(),
                        eventHandlerAttrs = implType.GetCustomAttributes<ClaptrapEventHandlerAttribute>().ToArray(),
                        eventStoreAttr = implType.GetCustomAttribute<EventStoreAttribute>(),
                        stateStoreAttr = implType.GetCustomAttribute<StateStoreAttribute>(),
                    };
                })
                .ToArray();

            var re = _claptrapDesignStoreFactory();

            var claptrapDesignTuples = metadata
                .Select(m =>
                {
                    var claptrapDesign = new ClaptrapDesign
                    {
                        Identity = new ClaptrapIdentity(string.Empty, GetActorTypeCode(m.stateAttr)),
                        StateDataType = m.stateAttr.StateDataType,
                        InitialStateDataFactoryType = m.stateInitialFactoryHandlerAttr.StateInitialFactoryHandlerType!,
                        StateHolderFactoryType = typeof(DeepClonerStateHolderFactory),
                        EventLoaderFactoryType = m.eventStoreAttr?.EventLoaderFactoryType!,
                        EventSaverFactoryType = m.eventStoreAttr?.EventSaverFactoryType!,
                        StateLoaderFactoryType = m.stateStoreAttr?.StateLoaderFactoryType!,
                        StateSaverFactoryType = m.stateStoreAttr?.StateSaverFactoryType!,
                        ClaptrapBoxInterfaceType = m.boxInterfaceType,
                        ClaptrapBoxImplementationType = m.boxImplType,
                        // TODO EventHandlerFactoryFactoryType 
                        // TODO state Options
                    };
                    ClaptrapEventHandlerDesign[] handlerDesigns;
                    //  find more event and handlers from attribute
                    handlerDesigns = m.eventAttrs.Length > m.eventHandlerAttrs.Length
                        ? m.eventAttrs.Select(e => new ClaptrapEventHandlerDesign
                            {
                                EventTypeCode = e.EventTypeCode,
                                EventDataType = e.EventDataType,
                                EventHandlerType = m.eventHandlerAttrs
                                    .FirstOrDefault(x => x.EventTypeCode == e.EventTypeCode)?.EventHandlerType!
                            })
                            .ToArray()
                        : m.eventHandlerAttrs.Select(e => new ClaptrapEventHandlerDesign
                            {
                                EventTypeCode = e.EventTypeCode,
                                EventDataType = m.eventAttrs
                                    .FirstOrDefault(x => x.EventTypeCode == e.EventTypeCode)?.EventDataType!,
                                EventHandlerType = e.EventHandlerType,
                            })
                            .ToArray();

                    claptrapDesign.EventHandlerDesigns =
                        new Dictionary<string, IClaptrapEventHandlerDesign>(
                            handlerDesigns.ToDictionary(a => a.EventTypeCode, a => (IClaptrapEventHandlerDesign) a));
                    return (claptrapDesign, m);
                })
                .ToArray();

            // try to map master and minions
            var typeCodeDic = claptrapDesignTuples
                .ToDictionary(x => x.claptrapDesign.Identity.TypeCode);

            foreach (var (claptrapDesign, m) in claptrapDesignTuples)
            {
                if (m.minionAttr != null)
                {
                    if (typeCodeDic.TryGetValue(m.minionAttr.MasterTypeCode, out var masterDesign))
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
        }
    }
}