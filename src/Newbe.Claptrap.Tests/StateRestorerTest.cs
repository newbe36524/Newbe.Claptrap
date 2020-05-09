using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Core.Impl;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class StateRestorerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StateRestorerTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private readonly IClaptrapIdentity _testClaptrapIdentity = new TestClaptrapIdentity("123", "testActor");

        [Fact]
        public async Task NoSnapshot()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
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

        [Fact]
        public async Task RestoreStateWithEmptyEvents()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);

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

        [Fact]
        public async Task RestoreStateWithSomeEvents()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);

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
            using var mocker = AutoMockHelper.Create(_testOutputHelper);

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

            await Assert.ThrowsAsync<Exception>(async () => await restorer.RestoreAsync());

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
    }
}