using System;
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
    public class MinionEventHandlerFLowTest
    {
        [Test]
        public async Task HandleNextVersionEvent()
        {
            IState state = new TestState
            {
                Version = 0
            };
            using var mocker = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    builder.RegisterBuildCallback(scope => scope.Resolve<IStateAccessor>().State = state);
                });

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.OnNewStateCreated(It.IsAny<IState>()));

            var flow = mocker.Create<MinionEventHandlerFLow>();

            flow.Activate();
            await flow.OnNewEventReceived(new TestEvent
            {
                Version = state.NextVersion
            });
            state.Version.Should().Be(1);
        }

        [Test]
        public async Task HandleVersionOlderEvent()
        {
            IState state = new TestState
            {
                Version = 1000
            };
            using var mocker = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    builder.RegisterBuildCallback(scope => scope.Resolve<IStateAccessor>().State = state);
                });

            var flow = mocker.Create<MinionEventHandlerFLow>();

            flow.Activate();
            await flow.OnNewEventReceived(new TestEvent
            {
                Version = 1
            });
            state.Version.Should().Be(1000,
                "do nothing as event version 1 lte state next version 1000 , skip the event");
            await flow.OnNewEventReceived(new TestEvent
            {
                Version = 1000
            });
            state.Version.Should().Be(1000,
                "do nothing as event version 1000 lte state next version 1000 , skip the event");
        }

        [Test]
        public async Task HandleVersionMoreThanNextVersionEvent()
        {
            IState state = new TestState
            {
                Version = 1000
            };
            using var mocker = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    builder.RegisterType<StateAccessor>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    builder.RegisterBuildCallback(scope => scope.Resolve<IStateAccessor>().State = state);
                    builder.RegisterInstance(new EventLoadingOptions
                    {
                        LoadingCountInOneBatch = 1000
                    });
                });

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .Setup(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.OnNewStateCreated(It.IsAny<IState>()));

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns<long, long>((left, right) => Task.FromResult(Enumerable
                    .Range((int) left, (int) right - (int) left)
                    .Select(i => new TestEvent
                    {
                        Version = i,
                    }).Cast<IEvent>()))
                .Callback<long, long>((left, right) => Console.WriteLine($"left {left} right {right}"));

            var flow = mocker.Create<MinionEventHandlerFLow>();

            flow.Activate();
            await flow.OnNewEventReceived(new TestEvent
            {
                Version = 1002
            });
            state.Version.Should().Be(1002,
                "do nothing as event version 1002 gt state next version 1000 , read event from event store");
            await flow.OnNewEventReceived(new TestEvent
            {
                Version = 2000
            });
            state.Version.Should().Be(2000,
                "do nothing as event version 2000 gt state next version 1000 , read event from event store");
        }

        [Test]
        public void ThrowExceptionAsHandlerWorks()
        {
            IState state = new TestState
            {
                Version = 0
            };
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
                    builder.RegisterBuildCallback(scope => scope.Resolve<IStateAccessor>().State = state);
                });

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new ExceptionHandler());

            var flow = mocker.Create<MinionEventHandlerFLow>();

            flow.Activate();
            Assert.ThrowsAsync<Exception>(() => flow.OnNewEventReceived(new TestEvent
            {
                Version = state.NextVersion
            }));
            state.Version.Should().Be(0);
        }

        [Test]
        public void ThrowExceptionAsHandlerWorksAndRestoreFromStore()
        {
            IState state = new TestState
            {
                Version = 0
            };
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
                    builder.RegisterBuildCallback(scope => scope.Resolve<IStateAccessor>().State = state);
                });

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .SetupSequence(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new ExceptionHandler());

            mocker.Mock<IStateRestorer>()
                .Setup(x => x.RestoreAsync())
                .Returns(Task.CompletedTask);

            var flow = mocker.Create<MinionEventHandlerFLow>();

            flow.Activate();
            Assert.ThrowsAsync<Exception>(() => flow.OnNewEventReceived(new TestEvent
            {
                Version = state.NextVersion
            }));
            state.Version.Should().Be(0);
        }
    }
}