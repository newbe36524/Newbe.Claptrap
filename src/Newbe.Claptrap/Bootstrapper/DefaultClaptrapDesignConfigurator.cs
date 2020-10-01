using System;
using System.Diagnostics;
using System.Linq;
using Newbe.Claptrap.EventNotifier;
using Newbe.Claptrap.StateHolder;

namespace Newbe.Claptrap.Bootstrapper
{
    public class DefaultClaptrapDesignConfigurator : IClaptrapDesignStoreConfigurator
    {
        private IClaptrapDesignStore? _designStore;

        public void Configure(IClaptrapDesignStore designStore)
        {
            _designStore = designStore;
            AddConfig(
                x => x.ClaptrapOptions.StateSavingOptions == null!,
                x => x.ClaptrapOptions.StateSavingOptions = new StateSavingOptions
                {
                    SavingWindowTime = TimeSpan.FromSeconds(10),
                    SaveWhenDeactivateAsync = true,
                    SavingWindowVersionLimit = 1000
                });
            AddConfig(
                x => x.ClaptrapOptions.MinionActivationOptions == null!,
                x => x.ClaptrapOptions.MinionActivationOptions = new MinionActivationOptions
                {
                    ActivateMinionsAtMasterStart = false
                });
            AddConfig(
                x => x.ClaptrapOptions.EventLoadingOptions == null!,
                x => x.ClaptrapOptions.EventLoadingOptions = new EventLoadingOptions
                {
                    LoadingCountInOneBatch = 1000
                });
            AddConfig(
                x => x.ClaptrapOptions.StateRecoveryOptions == null!,
                x => x.ClaptrapOptions.StateRecoveryOptions = new StateRecoveryOptions
                {
                    StateRecoveryStrategy = StateRecoveryStrategy.FromStore
                });
            AddConfig(
                x => x.InitialStateDataFactoryType == null,
                x => x.InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory));
            AddConfig(
                x => x.StateHolderFactoryType == null,
                x => x.StateHolderFactoryType = typeof(NoChangeStateHolderFactory));
            AddConfig(
                x => x.EventHandlerFactoryFactoryType == null,
                x => x.EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory));
            AddConfig(
                x => x.EventNotifierFactoryType == null,
                x => x.EventNotifierFactoryType = typeof(CompoundEventNotifierFactory));
            AddConfig(
                x => true,
                DisplayInfoFiller.FillDisplayInfo);
            AddConfig(
                x => x.ClaptrapOptions.EventCenterOptions == null!,
                x => x.ClaptrapOptions.EventCenterOptions = new EventCenterOptions
                {
                    EventCenterType = EventCenterType.None
                });
        }

        private void AddConfig(Func<IClaptrapDesign, bool> predicate,
            Action<IClaptrapDesign> action)
        {
            Debug.Assert(_designStore != null, nameof(_designStore) + " != null");
            foreach (var claptrapDesign in _designStore.Where(predicate))
            {
                action(claptrapDesign);
            }
        }
    }
}