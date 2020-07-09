using System;
using System.Collections.Concurrent;
using System.Linq;
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
            IEvent saveEvent = null;
            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .Returns(Task.CompletedTask)
                .Callback<IEvent>(e => saveEvent = e);

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
            saveEvent.Version.Should().Be(state.Version);
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        public async Task HandleEventConcurrently(int count)
        {
            using var mocker = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateRecoveryOptions());
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var state = new TestState
            {
                Data = new TestStateData
                {
                    Counter = 0
                }
            };
            mocker.Create<StateAccessor>().State = state;
            var savedEvents = new ConcurrentBag<IEvent>();
            mocker.Mock<IEventSaver>()
                .Setup(x => x.SaveEventAsync(It.IsAny<IEvent>()))
                .Returns(Task.CompletedTask)
                .Callback<IEvent>(e => savedEvents.Add(e));

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .Setup(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.OnNewEventHandled(It.IsAny<IEventNotifierContext>()));

            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.OnNewStateCreated(It.IsAny<IState>()));

            var flow = mocker.Create<MasterEventHandlerFLow>();

            flow.Activate();
            await Task.WhenAll(Enumerable.Range(0, count).Select(x => flow.OnNewEventReceived(new TestEvent())));
            state.Version.Should().Be(count);
            ((TestStateData) state.Data).Counter.Should().Be(count);
            savedEvents.Count.Should().Be(count);
            savedEvents
                .Select(x => x.Version)
                .OrderBy(x => x)
                .Should()
                .BeEquivalentTo(Enumerable.Range(Defaults.EventStartingVersion, count));
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