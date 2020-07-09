using System;
using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.Core;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapActorTest
    {
        [Test]
        public async Task ActivateAsync()
        {
            using var mocker = AutoMockHelper.Create();
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

        [Test]
        public void ActivateAsyncWithException()
        {
            var identity = TestClaptrapIdentity.Instance;
            using var mocker = AutoMockHelper.Create(builderAction: builder =>
            {
                builder.RegisterInstance(identity)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });
            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.Activate());
            mocker.Mock<IStateRestorer>()
                .Setup(x => x.RestoreAsync())
                .Returns(() => throw new Exception("some exception"));
            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            Assert.ThrowsAsync<ActivateFailException>(() => claptrap.ActivateAsync());
        }

        [Test]
        public async Task DeactivateAsync()
        {
            using var mocker = AutoMockHelper.Create();
            mocker.Mock<IEventHandledNotificationFlow>()
                .Setup(x => x.Deactivate());
            mocker.Mock<IStateSavingFlow>()
                .Setup(x => x.Deactivate());
            mocker.Mock<IEventHandlerFLow>()
                .Setup(x => x.Deactivate());
            IClaptrap claptrap = mocker.Create<ClaptrapActor>();
            await claptrap.DeactivateAsync();
        }

        [Test]
        public void DeactivateAndSavingState()
        {
            using var mocker = AutoMockHelper.Create(
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
            claptrap.DeactivateAsync();
        }
    }
}