using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Core.Impl;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class StateRestorerTest
    {
        private readonly IClaptrapIdentity _testClaptrapIdentity = new TestClaptrapIdentity("123", "testActor");

        [Test]
        public async Task NoSnapshot()
        {
            using var mocker = AutoMockHelper.Create();
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(default(IState));

            mocker.Mock<IInitialStateDataFactory>()
                .Setup(x => x.Create(It.IsAny<IClaptrapIdentity>()))
                .ReturnsAsync(new NoneStateData());

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IStateAccessor>()
                .SetupProperty(x => x.State);

            var restorer = mocker.Create<StateRestorer>();
            await restorer.RestoreAsync();
        }

        [Test]
        public async Task RestoreStateWithEmptyEvents()
        {
            using var mocker = AutoMockHelper.Create();

            var state = new TestState();
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IEventLoader>()
                .Setup(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<IEvent>());
            mocker.Mock<IStateAccessor>()
                .SetupProperty(x => x.State);

            var restorer = mocker.Create<StateRestorer>();
            await restorer.RestoreAsync();
        }

        [Test]
        public async Task RestoreStateWithSomeEvents()
        {
            using var mocker = AutoMockHelper.Create();

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
            mocker.Mock<IStateAccessor>()
                .SetupProperty(x => x.State);

            var restorer = mocker.Create<StateRestorer>();
            await restorer.RestoreAsync();

            state.Version.Should().Be(3);

            static IEnumerable<IEvent> AllEvents()
            {
                yield return new TestEvent
                {
                    Version = 1
                };
                yield return new TestEvent
                {
                    Version = 2
                };
                yield return new TestEvent
                {
                    Version = 3
                };
            }
        }

        [Test]
        public async Task RestoreStateWithSomeEvents_EventDelayReturn()
        {
            using var mocker = AutoMockHelper.Create();

            var state = new TestState();
            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(state);

            mocker.Mock<IEventLoader>()
                .SetupSequence(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    return AllEvents();
                }))
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IEventHandlerFactory>()
                .Setup(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());
            mocker.Mock<IStateAccessor>()
                .SetupProperty(x => x.State);

            var restorer = mocker.Create<StateRestorer>();
            await restorer.RestoreAsync();

            state.Version.Should().Be(3);

            static IEnumerable<IEvent> AllEvents()
            {
                yield return new TestEvent
                {
                    Version = 1
                };
                yield return new TestEvent
                {
                    Version = 2
                };
                yield return new TestEvent
                {
                    Version = 3
                };
            }
        }

        [Test]
        public async Task RestoreStateWithSomeEvents_EmptySnapShot()
        {
            IStateAccessor accessor = null;
            using var mocker = AutoMockHelper.Create(builderAction: builder =>
            {
                builder.RegisterType<StateAccessor>()
                    .As<IStateAccessor>()
                    .SingleInstance();
                builder.RegisterBuildCallback(container => { accessor = container.Resolve<IStateAccessor>(); });
            });

            mocker.Mock<IInitialStateDataFactory>()
                .Setup(x => x.Create(It.IsAny<IClaptrapIdentity>()))
                .ReturnsAsync(new NoneStateData());

            mocker.Mock<IStateLoader>()
                .Setup(x => x.GetStateSnapshotAsync())
                .ReturnsAsync(default(IState));

            mocker.Mock<IEventLoader>()
                .SetupSequence(x => x.GetEventsAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(AllEvents())
                .ReturnsAsync(Enumerable.Empty<IEvent>());

            mocker.Mock<IEventHandlerFactory>()
                .Setup(x => x.Create(It.IsAny<IEventContext>()))
                .Returns(new TestHandler());
            var restorer = mocker.Create<StateRestorer>();
            await restorer.RestoreAsync();

            accessor.State.Version.Should().Be(3);

            static IEnumerable<IEvent> AllEvents()
            {
                yield return new TestEvent
                {
                    Version = 1
                };
                yield return new TestEvent
                {
                    Version = 2
                };
                yield return new TestEvent
                {
                    Version = 3
                };
            }
        }

        [Test]
        public void RestoreWithThrowException()
        {
            using var mocker = AutoMockHelper.Create();

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
            mocker.Mock<IStateAccessor>()
                .SetupProperty(x => x.State);

            var restorer = mocker.Create<StateRestorer>();

            Assert.ThrowsAsync<Exception>(async () => await restorer.RestoreAsync());

            static IEnumerable<IEvent> AllEvents()
            {
                yield return new TestEvent
                {
                    Version = 1
                };
                yield return new TestEvent
                {
                    Version = 2
                };
                yield return new TestEvent
                {
                    Version = 3
                };
            }
        }
    }
}