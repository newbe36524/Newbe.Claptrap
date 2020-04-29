using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;
using Newbe.Claptrap.Preview.Impl;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapActorTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ClaptrapActorTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task NoSnapshot()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(default(IState));

            mocker.Mock<IInitialStateDataFactory>()
                .Setup(x => x.Create(It.IsAny<IClaptrapIdentity>()))
                .ReturnsAsync(new NoneStateData());

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.ActivateAsync();
        }

        [Fact]
        public async Task EmptyEvents()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            var state = new TestState();
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.ActivateAsync();
        }

        [Fact]
        public async Task RestoreStateWithSomeEvents()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            var state = new TestState();
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IEventLoader>()
                .SetupSequence(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(AllEvents())
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IEventHandlerFactory>()
                .Setup(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.ActivateAsync();

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
        public async Task RestoreWithThrowException()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            var state = new TestState
            {
                Identity = _testClaptrapIdentity
            };
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IEventLoader>()
                .SetupSequence(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(AllEvents())
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler())
                .Returns(new ExceptionHandler())
                .Returns(new TestHandler())
                ;

            IClaptrap claptrap = mocker.Create<ClaptrapActor>();

            await Assert.ThrowsAsync<ActivateFailException>(async () => await claptrap.ActivateAsync());

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
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.RegisterInstance(new StateSavingOptions
                    {
                        SavingWindowVersionLimit = 1
                    })
                    .SingleInstance();
            });
            mocker.VerifyAll = true;

            var state = new TestState();
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IStateSaver>()
                .Setup(x => x.SaveAsync(It.IsAny<IState>()))
                .Returns(Task.CompletedTask);

            mocker.Mock<IEventLoader>()
                .SetupSequence(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.Success);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler())
                ;

            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.ActivateAsync();

            await claptrap.HandleEvent(new TestEvent());
            state.Version.Should().Be(1);
        }

        [Fact]
        public async Task DeactivateAsync()
        {
            using var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            mocker.VerifyAll = true;

            var state = new TestState();
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.ActivateAsync();
            await claptrap.DeactivateAsync();
        }


        [Fact]
        public async Task DeactivateAndSavingState()
        {
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.RegisterInstance(new StateSavingOptions
                {
                    SaveWhenDeactivateAsync = true
                });
            });
            mocker.VerifyAll = true;

            var state = new TestState();
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IStateSaver>()
                .Setup(x => x.SaveAsync(It.IsAny<IState>()))
                .Returns(Task.CompletedTask);

            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.ActivateAsync();
            await claptrap.DeactivateAsync();
        }

        private readonly IClaptrapIdentity _testClaptrapIdentity = new ClaptrapIdentity("123", "testActor");
    }
}