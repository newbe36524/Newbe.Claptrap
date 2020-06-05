using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                .Where(x => x.GetInterface(nameof(IClaptrapBox)) != null)
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
                            implType?.GetCustomAttribute<ClaptrapStateInitialFactoryHandlerAttribute>(),
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
            var metadataDic = metadata
                .ToDictionary(m => GetActorTypeCode(m.stateAttr));

            var claptrapDesigns = metadata
                .Select(m => new ClaptrapDesign
                {
                    ClaptrapTypeCode = GetActorTypeCode(m.stateAttr)
                })
                .ToDictionary(x => x.ClaptrapTypeCode);

            // try to map master and minions
            foreach (var (typeCode, design) in claptrapDesigns)
            {
                var m = metadataDic[typeCode];
                if (m.minionAttr != null &&
                    claptrapDesigns.TryGetValue(m.minionAttr.MasterTypeCode, out var masterDesign))
                {
                    design.ClaptrapMasterDesign = masterDesign;
                }
            }

            foreach (var design in claptrapDesigns.Values)
            {
                var m = metadataDic[design.ClaptrapTypeCode];
                design.StateDataType = m.stateAttr.StateDataType;
                design.InitialStateDataFactoryType =
                    m.stateInitialFactoryHandlerAttr?.StateInitialFactoryHandlerType!;
                design.StateHolderFactoryType = m.stateHolderAttr?.StateHolderFactory!;
                design.EventLoaderFactoryType = m.eventStoreAttr?.EventLoaderFactoryType!;
                design.EventSaverFactoryType = m.eventStoreAttr?.EventSaverFactoryType!;
                design.StateLoaderFactoryType = m.stateStoreAttr?.StateLoaderFactoryType!;
                design.StateSaverFactoryType = m.stateStoreAttr?.StateSaverFactoryType!;
                design.ClaptrapBoxInterfaceType = m.boxInterfaceType;
                design.ClaptrapBoxImplementationType = m.boxImplType;
                design.ClaptrapOptions = new ClaptrapOptions
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
                };
                // TODO EventHandlerFactoryFactoryType 
                // find more event from attribute
                // attribute will be found on master design 
                ClaptrapEventAttribute[] eventAttrs = design.ClaptrapMasterDesign != null
                    ? metadataDic[design.ClaptrapMasterDesign.ClaptrapTypeCode].eventAttrs
                    : m.eventAttrs;

                var handlerDesigns = eventAttrs.Select(e => new ClaptrapEventHandlerDesign
                    {
                        EventTypeCode = e.EventTypeCode,
                        EventDataType = e.EventDataType,
                        EventHandlerType = m.eventHandlerAttrs
                            .FirstOrDefault(x => x.EventTypeCode == e.EventTypeCode)?.EventHandlerType!
                    })
                    .ToArray();

                design.EventHandlerDesigns =
                    new Dictionary<string, IClaptrapEventHandlerDesign>(
                        handlerDesigns.ToDictionary(a => a.EventTypeCode, a => (IClaptrapEventHandlerDesign) a));
            }

            foreach (var claptrapDesign in claptrapDesigns.Values)
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