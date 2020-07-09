using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class AopClaptrapTest
    {
        [Test]
        public async Task ActivateAsync()
        {
            using var mocker = AutoMockHelper.Create();
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

        [Test]
        public void ActivateAsyncException()
        {
            using var mocker = AutoMockHelper.Create();
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
            Assert.ThrowsAsync<Exception>(() => claptrap.ActivateAsync());
        }

        [Test]
        public async Task DeactivateAsync()
        {
            using var mocker = AutoMockHelper.Create();
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

        [Test]
        public void DeactivateAsyncException()
        {
            using var mocker = AutoMockHelper.Create();
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
            Assert.ThrowsAsync<Exception>(() => claptrap.DeactivateAsync());
        }

        [Test]
        public async Task HandleEventAsync()
        {
            using var mocker = AutoMockHelper.Create();
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

        [Test]
        public void HandleEventAsyncException()
        {
            using var mocker = AutoMockHelper.Create();
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
            Assert.ThrowsAsync<Exception>(() => claptrap.HandleEventAsync(testEvent));
        }
        
        [Test]
        public async Task HandleEventAsyncWithInterceptorException()
        {
            using var mocker = AutoMockHelper.Create();
            var testEvent = new TestEvent();
            mocker.Mock<IClaptrapLifetimeInterceptor>()
                .Setup(x => x.HandlingEventAsync(testEvent))
                .Returns(Task.FromException<Exception>(new Exception()));
            mocker.Mock<IClaptrap>()
                .Setup(x => x.HandleEventAsync(testEvent))
                .Returns(Task.CompletedTask);
            var claptrap = mocker.Create<AopClaptrap>();
            await claptrap.HandleEventAsync(testEvent);
        }
    }
}