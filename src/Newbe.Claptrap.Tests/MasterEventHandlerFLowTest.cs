using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;
using Newbe.Claptrap.Preview.Impl;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class MasterEventHandlerFLowTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MasterEventHandlerFLowTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task HandleEvent()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper,
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue
                    });
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState();
            mocker.Create<StateAccessor>().State = state;

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.Success);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());
            
            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.OnNewEventHandled(It.IsAny<IEventHandledNotifierContext>()));

            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.OnNewStateCreated(It.IsAny<IState>()));

            var flow = mocker.Create<MasterEventHandlerFLow>();

            flow.Activate();
            await flow.OnNewEventReceived(new TestEvent());
            state.Version.Should().Be(1);
        }

        [Fact]
        public async Task EventSavingResultAlreadyAdded()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper,
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue
                    });
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState();
            mocker.Create<StateAccessor>().State = state;
            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.AlreadyAdded);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            var flow = mocker.Create<MasterEventHandlerFLow>();

            flow.Activate();
            await flow.OnNewEventReceived(new TestEvent());
            state.Version.Should().Be(0);
        }

        [Fact]
        public async Task ThrownExceptionAsSavingEvent()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper,
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue
                    });
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState();
            mocker.Create<StateAccessor>().State = state;

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

            var flow = mocker.Create<MasterEventHandlerFLow>();

            flow.Activate();
            await Assert.ThrowsAsync<EventSavingException>(() => flow.OnNewEventReceived(new TestEvent
            {
                ClaptrapIdentity = TestClaptrapIdentity.Instance
            }));
            state.Version.Should().Be(0);
        }

        [Fact]
        public async Task ThrowExceptionAsHandlerWorks()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper,
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue,
                        StateRecoveryStrategy = StateRecoveryStrategy.FromStateHolder,
                    });
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState();
            mocker.Create<StateAccessor>().State = state;

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.Success);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new ExceptionHandler());

            var flow = mocker.Create<MasterEventHandlerFLow>();

            flow.Activate();
            await Assert.ThrowsAsync<Exception>(() => flow.OnNewEventReceived(new TestEvent()));
            state.Version.Should().Be(0);
        }

        [Fact]
        public async Task ThrowExceptionAsHandlerWorksAndRestoreFromStore()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper,
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateOptions
                    {
                        SavingWindowVersionLimit = int.MaxValue,
                        StateRecoveryStrategy = StateRecoveryStrategy.FromStore,
                    });
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState();
            mocker.Create<StateAccessor>().State = state;

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .ReturnsAsync(EventSavingResult.Success);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new ExceptionHandler());

            mocker.Mock<IStateRestorer>()
                .Setup(x => x.RestoreAsync())
                .Returns(Task.CompletedTask);

            var flow = mocker.Create<MasterEventHandlerFLow>();

            flow.Activate();
            await Assert.ThrowsAsync<Exception>(() => flow.OnNewEventReceived(new TestEvent()));
            state.Version.Should().Be(0);
        }
    }
}