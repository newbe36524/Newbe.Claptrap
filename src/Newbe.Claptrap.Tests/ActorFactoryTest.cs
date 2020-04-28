using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.StateStore;
using Xunit;

namespace Newbe.Claptrap.Tests
{
    public class ActorFactoryTest
    {
        [Fact]
        public void Create()
        {
            using var mocker = AutoMock.GetStrict();
            mocker.VerifyAll = true;

            var actorIdentity = ActorIdentity.Instance;
            mocker.Mock<IStateStoreFactory>()
                .Setup(x => x.Create(actorIdentity))
                .Returns(new MemoryStateStore(actorIdentity, default!));
            
            mocker.Mock<IEventStoreFactory>()
                .Setup(x => x.Create(actorIdentity))
                .Returns(new MemoryEventStore(actorIdentity));

            var actorFactory = mocker.Create<ActorFactory>();
            var actor = actorFactory.Create(actorIdentity);
            actor.Should().NotBeNull();
        }
    }
}