using System;
using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;
using Newbe.Claptrap.Preview.Abstractions.Options;
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
        public async Task ActivateAsync()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.Activate());
            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.Activate());
            mocker.Mock<IEventHandlerFLow>()
                .Setup(x => x.Activate());
            mocker.Mock<IStateRestorer>()
                .Setup(x => x.RestoreAsync())
                .Returns(Task.CompletedTask);
            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.ActivateAsync();
        }

        [Fact]
        public async Task ActivateAsyncWithException()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.Activate());
            mocker.Mock<IStateRestorer>()
                .Setup(x => x.RestoreAsync())
                .Returns(() => throw new Exception("some exception"));
            mocker.Mock<IStateAccessor>()
                .SetupGet(x => x.State)
                .Returns(new TestState
                {
                    Identity = TestClaptrapIdentity.Instance
                });
            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await Assert.ThrowsAsync<ActivateFailException>(() => claptrap.ActivateAsync());
        }

        [Fact]
        public async Task DeactivateAsync()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.Deactivate());
            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.Deactivate());
            mocker.Mock<IEventHandlerFLow>()
                .Setup(x => x.Deactivate());
            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.DeactivateAsync();
        }

        [Fact]
        public async Task DeactivateAndSavingState()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper,
                builderAction: builder =>
                {
                    builder.RegisterInstance(new StateSavingOptions
                    {
                        SaveWhenDeactivateAsync = true
                    });
                });
            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.Deactivate());
            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.Deactivate());
            mocker.Mock<IEventHandlerFLow>()
                .Setup(x => x.Deactivate());

            var testState = new TestState();
            mocker.Mock<IStateAccessor>()
                .SetupGet(x => x.State)
                .Returns(testState);
            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.SaveStateAsync(testState))
                .Returns(Task.CompletedTask);

            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.DeactivateAsync();
        }
    }
}