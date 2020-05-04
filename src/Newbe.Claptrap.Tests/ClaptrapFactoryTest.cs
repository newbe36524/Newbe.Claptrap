using System.Collections.Immutable;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Impl;
using Newbe.Claptrap.Preview.Impl.MemoryStore;
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
        public void Create()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);

            var actorIdentity = TestClaptrapIdentity.Instance;
            mocker.Mock<IClaptrapDesignStore>()
                .Setup(x => x.FindDesign(actorIdentity))
                .Returns(new ClaptrapDesign
                {
                    StateOptions = new StateOptions(),
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
                });

            var actorFactory = mocker.Create<ClaptrapFactory>();
            var actor = actorFactory.Create(actorIdentity);
            actor.Should().NotBeNull();
        }

        private class TestStateData : IStateData
        {
        }
    }
}