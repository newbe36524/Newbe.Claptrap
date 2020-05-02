using System;
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

        #region RestoreState

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

            var claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.RestoreStateAsync();
        }

        [Fact]
        public async Task RestoreStateWithEmptyEvents()
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

            var claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.RestoreStateAsync();
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

            var claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.RestoreStateAsync();

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

            var claptrap = mocker.Create<ClaptrapActor>();

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

        #endregion

        #region Deactivate

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
                builder.RegisterInstance(new StateOptions
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

        #endregion


        #region HandleEvent

        [Fact]
        public async Task HandleEvent()
        {
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue
                    })
                    .SingleInstance();
            });
            mocker.VerifyAll = true;

            var state = new TestState();

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.Success);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            var claptrap = mocker.Create<ClaptrapActor>();
            claptrap.State = state;
            claptrap.CreateFlows();

            await claptrap.HandleEvent(new TestEvent());
            state.Version.Should().Be(1);
        }

        [Fact]
        public async Task EventSavingResultAlreadyAdded()
        {
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue
                    })
                    .SingleInstance();
            });
            mocker.VerifyAll = true;

            var state = new TestState();

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.AlreadyAdded);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            var claptrap = mocker.Create<ClaptrapActor>();
            claptrap.State = state;
            claptrap.CreateFlows();

            await claptrap.HandleEvent(new TestEvent());
            state.Version.Should().Be(0);
        }

        [Fact]
        public async Task ThrownExceptionAsSavingEvent()
        {
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue
                    })
                    .SingleInstance();
            });
            mocker.VerifyAll = true;

            var state = new TestState();

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .Returns<IEvent>(@event =>
                {
                    var ex = new EventSavingException(new Exception("saving with exception"), @event);
                    throw ex;
                });

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            var claptrap = mocker.Create<ClaptrapActor>();
            claptrap.State = state;
            claptrap.CreateFlows();

            await Assert.ThrowsAsync<EventSavingException>(() => claptrap.HandleEvent(new TestEvent
            {
                ClaptrapIdentity = TestClaptrapIdentity.Instance
            }));
            state.Version.Should().Be(0);
        }

        [Fact]
        public async Task ThrowExceptionAsHandlerWorks()
        {
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue,
                        StateRecoveryStrategy = StateRecoveryStrategy.FromStateHolder,
                    })
                    .SingleInstance();
            });
            mocker.VerifyAll = true;

            var state = new TestState();

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.Success);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new ExceptionHandler());

            var claptrap = mocker.Create<ClaptrapActor>();
            claptrap.State = state;
            claptrap.CreateFlows();

            await Assert.ThrowsAsync<Exception>(() => claptrap.HandleEvent(new TestEvent()));
            state.Version.Should().Be(0);
        }

        [Fact]
        public async Task ThrowExceptionAsHandlerWorksAndRestoreFromStore()
        {
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue,
                        StateRecoveryStrategy = StateRecoveryStrategy.FromStore,
                    })
                    .SingleInstance();
            });
            mocker.VerifyAll = true;

            var state = new TestState();

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.Success);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new ExceptionHandler());

            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            var claptrap = mocker.Create<ClaptrapActor>();
            claptrap.State = state;
            claptrap.CreateFlows();

            await Assert.ThrowsAsync<Exception>(() => claptrap.HandleEvent(new TestEvent()));
            state.Version.Should().Be(0);
        }

        #endregion

        private readonly IClaptrapIdentity _testClaptrapIdentity = new TestClaptrapIdentity("123", "testActor");
    }
}