using System.Collections.Immutable;
using System.Linq;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Design;
using Newbe.Claptrap.MemoryStore;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapFactoryTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ClaptrapFactoryTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateMaster()
        {
            var actorIdentity = TestClaptrapIdentity.Instance;
            var claptrapDesign = new ClaptrapDesign
            {
                ClaptrapOptions = new ClaptrapOptions
                {
                    MinionActivationOptions = new MinionActivationOptions(),
                    EventLoadingOptions = new EventLoadingOptions(),
                    StateRecoveryOptions = new StateRecoveryOptions(),
                    StateSavingOptions = new StateSavingOptions()
                },
                Identity = actorIdentity,
                EventHandlerDesigns = ImmutableDictionary<string, IClaptrapEventHandlerDesign>.Empty,
                StateHolderFactoryType = typeof(DeepClonerStateHolderFactory),
                StateDataType = typeof(TestStateData),
                EventLoaderFactoryType = typeof(MemoryEventStoreFactory),
                EventSaverFactoryType = typeof(MemoryEventStoreFactory),
                StateLoaderFactoryType = typeof(MemoryStateStoreFactory),
                StateSaverFactoryType = typeof(MemoryStateStoreFactory),
                EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory),
                InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory)
            };
            var claptrapDesignStore = new ClaptrapDesignStore();
            claptrapDesignStore.AddOrReplace(claptrapDesign);
            using var mocker = AutoMockHelper.Create(_testOutputHelper, builderAction: builder =>
            {
                builder.RegisterInstance(claptrapDesignStore)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });

            mocker.Mock<IClaptrapModuleProvider>()
                .Setup(x => x.GetClaptrapSharedModules(actorIdentity))
                .Returns(Enumerable.Empty<IClaptrapSharedModule>());
            mocker.Mock<IClaptrapModuleProvider>()
                .Setup(x => x.GetClaptrapMasterClaptrapModules(actorIdentity))
                .Returns(Enumerable.Empty<IClaptrapMasterModule>());

            var actorFactory = mocker.Create<ClaptrapFactory>();
            var actor = actorFactory.Create(actorIdentity);
            actor.Should().NotBeNull();
        }

        [Fact]
        public void CreateMinion()
        {
            var actorIdentity = TestClaptrapIdentity.Instance;
            var claptrapDesignStore = new ClaptrapDesignStore();
            var masterDesign = new ClaptrapDesign
            {
                ClaptrapOptions = new ClaptrapOptions
                {
                    MinionActivationOptions = new MinionActivationOptions(),
                    EventLoadingOptions = new EventLoadingOptions(),
                    StateRecoveryOptions = new StateRecoveryOptions(),
                    StateSavingOptions = new StateSavingOptions()
                },
                Identity = actorIdentity,
                EventHandlerDesigns = ImmutableDictionary<string, IClaptrapEventHandlerDesign>.Empty,
                StateHolderFactoryType = typeof(DeepClonerStateHolderFactory),
                StateDataType = typeof(TestStateData),
                EventLoaderFactoryType = typeof(MemoryEventStoreFactory),
                EventSaverFactoryType = typeof(MemoryEventStoreFactory),
                StateLoaderFactoryType = typeof(MemoryStateStoreFactory),
                StateSaverFactoryType = typeof(MemoryStateStoreFactory),
                EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory),
                InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory)
            };
            claptrapDesignStore.AddOrReplace(masterDesign);
            var minionDesign = new ClaptrapDesign
            {
                ClaptrapOptions = new ClaptrapOptions
                {
                    MinionActivationOptions = new MinionActivationOptions(),
                    EventLoadingOptions = new EventLoadingOptions(),
                    StateRecoveryOptions = new StateRecoveryOptions(),
                    StateSavingOptions = new StateSavingOptions()
                },
                Identity = actorIdentity,
                EventHandlerDesigns = ImmutableDictionary<string, IClaptrapEventHandlerDesign>.Empty,
                StateHolderFactoryType = typeof(DeepClonerStateHolderFactory),
                StateDataType = typeof(TestStateData),
                EventLoaderFactoryType = typeof(MemoryEventStoreFactory),
                EventSaverFactoryType = typeof(MemoryEventStoreFactory),
                StateLoaderFactoryType = typeof(MemoryStateStoreFactory),
                StateSaverFactoryType = typeof(MemoryStateStoreFactory),
                EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory),
                InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory),
                ClaptrapMasterDesign = masterDesign
            };
            claptrapDesignStore.AddOrReplace(minionDesign);
            using var mocker = AutoMockHelper.Create(_testOutputHelper, builderAction: builder =>
            {
                builder.RegisterInstance(claptrapDesignStore)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });

            mocker.Mock<IClaptrapModuleProvider>()
                .Setup(x => x.GetClaptrapSharedModules(actorIdentity))
                .Returns(Enumerable.Empty<IClaptrapSharedModule>());
            mocker.Mock<IClaptrapModuleProvider>()
                .Setup(x => x.GetClaptrapMinionModules(actorIdentity))
                .Returns(Enumerable.Empty<IClaptrapMinionModule>());

            var actorFactory = mocker.Create<ClaptrapFactory>();
            var actor = actorFactory.Create(actorIdentity);
            actor.Should().NotBeNull();
        }


        private class TestStateData : IStateData
        {
        }
    }
}