using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Xunit;
using Moq;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Context;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventHandler;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.StateStore;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class ActorTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ActorTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task NoSnapshot()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            mocker.Mock<IStateStore>()
                .Setup(x => x.GetStateSnapshot())
                .ReturnsAsync(default(IState));

            mocker.Mock<IInitialStateDataFactory>()
                .Setup(x => x.Create(It.IsAny<IActorIdentity>()))
                .ReturnsAsync(new NoneStateData());

            mocker.Mock<IEventStore>()
                .Setup(x => x.GetEvents(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            IActor actor = mocker.Create<Actor>();
            await actor.ActivateAsync();
        }

        [Fact]
        public async Task EmptyEvents()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            var state = new TestState();
            mocker.Mock<IStateStore>()
                .Setup(x => x.GetStateSnapshot())
                .ReturnsAsync(state);

            mocker.Mock<IEventStore>()
                .Setup(x => x.GetEvents(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            IActor actor = mocker.Create<Actor>();
            await actor.ActivateAsync();
        }

        [Fact]
        public async Task SomeEvents()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            var state = new TestState();
            mocker.Mock<IStateStore>()
                .Setup(x => x.GetStateSnapshot())
                .ReturnsAsync(state);

            mocker.Mock<IEventStore>()
                .SetupSequence(x => x.GetEvents(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(AllEvents())
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IEventHandlerFactory>()
                .Setup(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            IActor actor = mocker.Create<Actor>();
            await actor.ActivateAsync();

            state.Version.Should().Be(3);

            static IEnumerable<IEvent> AllEvents()
            {
                yield return new TestEvent
                {
                    Version = 1,
                };
                yield return new TestEvent
                {
                    Version = 2,
                };
                yield return new TestEvent
                {
                    Version = 3,
                };
            }
        }

        [Fact]
        public async Task ThrowException()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            var state = new TestState
            {
                Identity = _testActorIdentity
            };
            mocker.Mock<IStateStore>()
                .Setup(x => x.GetStateSnapshot())
                .ReturnsAsync(state);

            mocker.Mock<IEventStore>()
                .SetupSequence(x => x.GetEvents(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(AllEvents())
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler())
                .Returns(new ExceptionHandler())
                .Returns(new TestHandler())
                ;

            IActor actor = mocker.Create<Actor>();

            await Assert.ThrowsAsync<ActivateFailException>(async () => await actor.ActivateAsync());

            state.Version.Should().Be(1);

            static IEnumerable<IEvent> AllEvents()
            {
                yield return new TestEvent
                {
                    Version = 1,
                };
                yield return new TestEvent
                {
                    Version = 2,
                };
                yield return new TestEvent
                {
                    Version = 3,
                };
            }
        }

        [Fact]
        public async Task HandleEvent()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            var state = new TestState();
            mocker.Mock<IStateStore>()
                .Setup(x => x.GetStateSnapshot())
                .ReturnsAsync(state);
            
            mocker.Mock<IStateStore>()
                .Setup(x => x.Save(It.IsAny<IState>()))
                .Returns(Task.CompletedTask);

            mocker.Mock<IEventStore>()
                .SetupSequence(x => x.GetEvents(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IEventStore>()
                .Setup(x => x.SaveEvent(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.Success);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler())
                ;

            IActor actor = mocker.Create<Actor>();
            await actor.ActivateAsync();

            await actor.HandleEvent(new TestEvent());
            state.Version.Should().Be(1);
        }

        private readonly IActorIdentity _testActorIdentity = new ActorIdentity("123", "testActor");
    }
}