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

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapFactoryTest
    {
        [Fact]
        public void Create()
        {
            using var mocker = AutoMock.GetStrict();
            mocker.VerifyAll = true;

            var actorIdentity = ClaptrapIdentity.Instance;
            mocker.Mock<IClaptrapDesignStore>()
                .Setup(x => x.FindDesign(actorIdentity))
                .Returns(new ClaptrapDesign
                {
                    StateSavingOptions = new StateSavingOptions(),
                    Identity = actorIdentity,
                    EventHandlerDesigns = ImmutableDictionary<string, IClaptrapEventHandlerDesign>.Empty,
                    StateHolderFactoryType = typeof(DeepClonerStateHolderFactory),
                    ActorStateDataType = typeof(TestStateData),
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