using System;
using Newbe.Claptrap.EventNotifier;
using Newbe.Claptrap.StateHolder;

namespace Newbe.Claptrap.Bootstrapper
{
    public static class DefaultClaptrapDesignConfigurator
    {
        public static void Configure(IClaptrapBootstrapperBuilder builder)
        {
            builder
                .ConfigureClaptrapDesign(
                    x => x.ClaptrapOptions.StateSavingOptions == null,
                    x => x.ClaptrapOptions.StateSavingOptions = new StateSavingOptions
                    {
                        SavingWindowTime = TimeSpan.FromSeconds(10),
                        SaveWhenDeactivateAsync = true,
                        SavingWindowVersionLimit = 1000,
                    })
                .ConfigureClaptrapDesign(
                    x => x.ClaptrapOptions.MinionActivationOptions == null,
                    x => x.ClaptrapOptions.MinionActivationOptions = new MinionActivationOptions
                    {
                        ActivateMinionsAtMasterStart = false
                    })
                .ConfigureClaptrapDesign(
                    x => x.ClaptrapOptions.EventLoadingOptions == null,
                    x => x.ClaptrapOptions.EventLoadingOptions = new EventLoadingOptions
                    {
                        LoadingCountInOneBatch = 1000
                    })
                .ConfigureClaptrapDesign(
                    x => x.ClaptrapOptions.StateRecoveryOptions == null,
                    x => x.ClaptrapOptions.StateRecoveryOptions = new StateRecoveryOptions
                    {
                        StateRecoveryStrategy = StateRecoveryStrategy.FromStore
                    })
                .ConfigureClaptrapDesign(
                    x => x.InitialStateDataFactoryType == null,
                    x => x.InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory))
                .ConfigureClaptrapDesign(
                    x => x.StateHolderFactoryType == null,
                    x => x.StateHolderFactoryType = typeof(NoChangeStateHolderFactory))
                .ConfigureClaptrapDesign(
                    x => x.EventHandlerFactoryFactoryType == null,
                    x => x.EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory))
                .ConfigureClaptrapDesign(
                    x => x.EventNotifierFactoryType == null,
                    x => x.EventNotifierFactoryType = typeof(CompoundEventNotifierFactory));
            builder
                .ConfigureClaptrapDesign(
                    x => true,
                    DisplayInfoFiller.FillDisplayInfo)
                ;
        }
    }
}