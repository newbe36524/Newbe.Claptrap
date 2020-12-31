using System;
using System.Collections.Generic;
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
        public void MissingVersion()
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

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns<long, long>((left, right) => Task.FromResult(Enumerable.Empty<IEvent>()));

            var flow = mocker.Create<MinionEventHandlerFLow>();

            flow.Activate();
            const int handlingVersion = 1000;
            Assert.ThrowsAsync<VersionErrorException>(async () =>
            {
                await flow.OnNewEventReceived(new TestEvent
                {
                    Version = handlingVersion
                });
            }, $"should be thrown : missing version between {state.Version} and {handlingVersion}");
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
        public async Task ConcurrentMixOrder()
        {
            const int targetVersion = 10000;
            IState state = new TestState
            {
                Version = 500
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
            var sourceEvents = Enumerable.Range(0, targetVersion + 1).Select(x => new TestEvent
            {
                Version = x
            }).ToArray();
            var sendingEvents = sourceEvents.ToList();
            Shuffle(sendingEvents);

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns<long, long>((left, right) => Task.FromResult(sourceEvents
                    .Skip((int) left)
                    .Take((int) (right - left))
                    .Cast<IEvent>()))
                .Callback<long, long>((left, right) => Console.WriteLine($"left {left} right {right}"));

            mocker.Mock<IStateHolder>()
                .Setup(x => x.DeepCopy(It.IsAny<IState>()))
                .Returns(state);

            mocker.Mock<IEventHandlerFactory>()
                .Setup(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());

            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.OnNewStateCreated(It.IsAny<IState>()));

            var flow = mocker.Create<MinionEventHandlerFLow>();

            flow.Activate();

            await Task.WhenAll(sendingEvents.Select(flow.OnNewEventReceived));
            state.Version.Should().Be(targetVersion);
        }

        private static readonly Random Rd = new Random();

        private static void Shuffle<T>(IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rd.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
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
                        Version = i
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