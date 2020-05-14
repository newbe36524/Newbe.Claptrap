using System;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Core.Impl;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class MasterEventHandlerFLowTest
    {
        [Test]
        public async Task HandleEvent()
        {
            using var mocker = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateRecoveryOptions());
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState();
            mocker.Create<StateAccessor>().State = state;

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .Returns(Task.CompletedTask);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.OnNewEventHandled(It.IsAny<IEventNotifierContext>()));

            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.OnNewStateCreated(It.IsAny<IState>()));

            var flow = mocker.Create<MasterEventHandlerFLow>();

            flow.Activate();
            await flow.OnNewEventReceived(new TestEvent());
            state.Version.Should().Be(1);
        }

        [Test]
        public void ThrownExceptionAsSavingEvent()
        {
            using var mocker = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateRecoveryOptions());
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
            Assert.ThrowsAsync<EventSavingException>(() => flow.OnNewEventReceived(new TestEvent
            {
                ClaptrapIdentity = TestClaptrapIdentity.Instance
            }));
            state.Version.Should().Be(0);
        }

        [Test]
        public void ThrowExceptionAsHandlerWorks()
        {
            using var mocker = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateRecoveryOptions
                    {
                        StateRecoveryStrategy = StateRecoveryStrategy.FromStateHolder
                    });
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState();
            mocker.Create<StateAccessor>().State = state;

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .Returns(Task.CompletedTask);

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new ExceptionHandler());

            var flow = mocker.Create<MasterEventHandlerFLow>();

            flow.Activate();
            Assert.ThrowsAsync<Exception>(() => flow.OnNewEventReceived(new TestEvent()));
            state.Version.Should().Be(0);
        }

        [Test]
        public void ThrowExceptionAsHandlerWorksAndRestoreFromStore()
        {
            using var mocker = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateRecoveryOptions
                    {
                        StateRecoveryStrategy = StateRecoveryStrategy.FromStore
                    });
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState();
            mocker.Create<StateAccessor>().State = state;

            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .Returns(Task.CompletedTask);

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
            Assert.ThrowsAsync<Exception>(() => flow.OnNewEventReceived(new TestEvent()));
            state.Version.Should().Be(0);
        }
    }
}