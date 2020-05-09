using System;
using System.Threading.Tasks;
using Autofac;
using Moq;
using Newbe.Claptrap.Core.Impl;
using Newbe.Claptrap.Options;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class StateSavingFlowTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StateSavingFlowTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WindowVersionLimit1()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper, builderAction: builder =>
            {
                builder.RegisterInstance(new StateSavingOptions
                {
                    SavingWindowVersionLimit = 1
                });
            });

            mocker.Mock<IStateSaver>()
                .Setup(x => x.SaveAsync(It.IsAny<IState>()))
                .Returns(Task.CompletedTask);

            var stateSavingFlow = mocker.Create<StateSavingFlow>();
            stateSavingFlow.Activate();
            stateSavingFlow.OnNewStateCreated(new TestState());
        }

        [Fact]
        public void WindowVersionLimitTime1Seconds()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper, builderAction: builder =>
            {
                builder.RegisterInstance(new StateSavingOptions
                {
                    SavingWindowTime = TimeSpan.FromSeconds(1)
                });
            });

            var stateSavingFlow = mocker.Create<StateSavingFlow>();
            stateSavingFlow.Activate();
            stateSavingFlow.OnNewStateCreated(new TestState());

            // there will be nothing saved since it deactivate with 1 sec
        }

        [Fact]
        public void WindowVersionLimit2()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper, builderAction: builder =>
            {
                builder.RegisterInstance(new StateSavingOptions
                {
                    SavingWindowVersionLimit = 2
                });
            });
            var laterState = new TestState
            {
                Version = 1000
            };

            // setup only once, it means that this function can not be invoke twice
            mocker.Mock<IStateSaver>()
                .SetupSequence(x => x.SaveAsync(laterState))
                .Returns(Task.CompletedTask);

            var stateSavingFlow = mocker.Create<StateSavingFlow>();
            stateSavingFlow.Activate();

            // add two, by only save the later one
            stateSavingFlow.OnNewStateCreated(new TestState());
            stateSavingFlow.OnNewStateCreated(laterState);
        }

        [Fact]
        public void NotSave()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper, builderAction: builder =>
            {
                // there is no saving windows set, it will save nothing
                builder.RegisterInstance(new StateSavingOptions());
            });

            var stateSavingFlow = mocker.Create<StateSavingFlow>();
            stateSavingFlow.Activate();
            stateSavingFlow.OnNewStateCreated(new TestState());
        }


        [Fact]
        public void ExceptionAdnContinue()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper, builderAction: builder =>
            {
                builder.RegisterInstance(new StateSavingOptions
                {
                    SavingWindowVersionLimit = 1
                });
            });
            var laterState = new TestState
            {
                Version = 1000
            };

            mocker.Mock<IStateSaver>()
                .SetupSequence(x => x.SaveAsync(It.IsAny<IState>()))
                .Returns(Task.FromException(new Exception("something error while save state")))
                .Returns(Task.CompletedTask);

            var stateSavingFlow = mocker.Create<StateSavingFlow>();
            stateSavingFlow.Activate();

            // first will not save as there is a exception thrown
            stateSavingFlow.OnNewStateCreated(new TestState());

            // later one will be save as it is ok
            stateSavingFlow.OnNewStateCreated(laterState);
        }
    }
}