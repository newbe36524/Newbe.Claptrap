using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.Design
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
            if (types.Length == 0)
            {
                return _claptrapDesignStoreFactory();
            }

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
                        eventStoreAttr = implType.GetCustomAttribute<ClaptrapEventStoreAttribute>(),
                        stateStoreAttr = implType.GetCustomAttribute<ClaptrapStateStoreAttribute>(),
                        stateHolderAttr = implType.GetCustomAttribute<ClaptrapStateHolderAttribute>(),
                        StateSavingOptionsAttribute =
                            implType.GetCustomAttribute<ClaptrapStateSavingOptionsAttribute>(),
                        MinionOptionsAttribute =
                            implType.GetCustomAttribute<ClaptrapMinionOptionsAttribute>(),
                        EventLoadingOptionsAttribute =
                            implType.GetCustomAttribute<ClaptrapEventLoadingOptionsAttribute>(),
                        StateRecoveryOptionsAttribute =
                            implType.GetCustomAttribute<ClaptrapStateRecoveryOptionsAttribute>(),
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
                        StateHolderFactoryType = m.stateHolderAttr?.StateHolderFactory!,
                        EventLoaderFactoryType = m.eventStoreAttr?.EventLoaderFactoryType!,
                        EventSaverFactoryType = m.eventStoreAttr?.EventSaverFactoryType!,
                        StateLoaderFactoryType = m.stateStoreAttr?.StateLoaderFactoryType!,
                        StateSaverFactoryType = m.stateStoreAttr?.StateSaverFactoryType!,
                        ClaptrapBoxInterfaceType = m.boxInterfaceType,
                        ClaptrapBoxImplementationType = m.boxImplType,
                        ClaptrapOptions = new ClaptrapOptions
                        {
                            MinionActivationOptions = m.MinionOptionsAttribute == null
                                ? null!
                                : new MinionActivationOptions
                                {
                                    ActivateMinionsAtMasterStart = m.MinionOptionsAttribute.ActivateMinionsAtStart
                                },
                            EventLoadingOptions = m.EventLoadingOptionsAttribute == null
                                ? null!
                                : new EventLoadingOptions
                                {
                                    LoadingCountInOneBatch = m.EventLoadingOptionsAttribute.LoadingCountInOneBatch,
                                },
                            StateRecoveryOptions = m.StateRecoveryOptionsAttribute == null
                                ? null!
                                : new StateRecoveryOptions
                                {
                                    StateRecoveryStrategy = m.StateRecoveryOptionsAttribute.StateRecoveryStrategy
                                },
                            StateSavingOptions = m.StateSavingOptionsAttribute == null
                                ? null!
                                : new StateSavingOptions
                                {
                                    SavingWindowTime = m.StateSavingOptionsAttribute.SavingWindowTime,
                                    SavingWindowVersionLimit = m.StateSavingOptionsAttribute.SavingWindowVersionLimit,
                                    SaveWhenDeactivateAsync = m.StateSavingOptionsAttribute.SaveWhenDeactivateAsync,
                                }
                        },
                        // TODO EventHandlerFactoryFactoryType 
                    };
                    //  find more event and handlers from attribute
                    var handlerDesigns = m.eventAttrs.Length > m.eventHandlerAttrs.Length
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
                return attr.ClaptrapTypeCode ??
                       attr.StateDataType.FullName;
            }
        }
    }
}