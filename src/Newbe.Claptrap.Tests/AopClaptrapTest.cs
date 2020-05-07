using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Impl;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class AopClaptrapTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AopClaptrapTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ActivateAsync()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.ActivatingAsync())
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.ActivatedAsync())
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrap>()
                .Setup(x => x.ActivateAsync())
                .Returns(Task.CompletedTask);
            var claptrap = mocker.Create<AopClaptrap>();
            await claptrap.ActivateAsync();
        }

        [Fact]
        public async Task ActivateAsyncException()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.ActivatingAsync())
                .Returns(Task.CompletedTask);
            var exception = new Exception();
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.ActivatingThrowExceptionAsync(exception))
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrap>()
                .Setup(x => x.ActivateAsync())
                .Returns(Task.FromException<Exception>(exception));
            var claptrap = mocker.Create<AopClaptrap>();
            await Assert.ThrowsAsync<Exception>(() => claptrap.ActivateAsync());
        }

        [Fact]
        public async Task DeactivateAsync()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.DeactivatingAsync())
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.DeactivatedAsync())
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrap>()
                .Setup(x => x.DeactivateAsync())
                .Returns(Task.CompletedTask);
            var claptrap = mocker.Create<AopClaptrap>();
            await claptrap.DeactivateAsync();
        }

        [Fact]
        public async Task DeactivateAsyncException()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.DeactivatingAsync())
                .Returns(Task.CompletedTask);
            var exception = new Exception();
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.DeactivatingThrowExceptionAsync(exception))
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrap>()
                .Setup(x => x.DeactivateAsync())
                .Returns(Task.FromException<Exception>(exception));
            var claptrap = mocker.Create<AopClaptrap>();
            await Assert.ThrowsAsync<Exception>(() => claptrap.DeactivateAsync());
        }

        [Fact]
        public async Task HandleEventAsync()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            var testEvent = new TestEvent();
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.HandlingEventAsync(testEvent))
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.HandledEventAsync(testEvent))
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrap>()
                .Setup(x => x.HandleEventAsync(testEvent))
                .Returns(Task.CompletedTask);
            var claptrap = mocker.Create<AopClaptrap>();
            await claptrap.HandleEventAsync(testEvent);
        }

        [Fact]
        public async Task HandleEventAsyncException()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            var testEvent = new TestEvent();
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.HandlingEventAsync(testEvent))
                .Returns(Task.CompletedTask);
            var exception = new Exception();
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.HandlingEventThrowExceptionAsync(testEvent, exception))
                .Returns(Task.CompletedTask);
            mocker.Mock<IClaptrap>()
                .Setup(x => x.HandleEventAsync(testEvent))
                .Returns(Task.FromException<Exception>(exception));
            var claptrap = mocker.Create<AopClaptrap>();
            await Assert.ThrowsAsync<Exception>(() => claptrap.HandleEventAsync(testEvent));
        }
    }
}