using System;
using System.Threading.Tasks;
using Moq;
using Newbe.Claptrap.Core.Impl;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class EventHandledNotificationFlowTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EventHandledNotificationFlowTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void OnNewEventHandled()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IEventNotifier>()
                .Setup(x => x.Notify(It.IsAny<IEventNotifierContext>()))
                .Returns(Task.CompletedTask);
            var flow = mocker.Create<EventHandledNotificationFlow>();
            flow.Activate();
            flow.OnNewEventHandled(new EventNotifierContext
            {
                Event = new TestEvent(),
                CurrentState = new TestState(),
                OldState = new TestState()
            });
        }

        [Fact]
        public void OnNewEventHandled_NotifierException()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            mocker.Mock<IEventNotifier>()
                .Setup(x => x.Notify(It.IsAny<IEventNotifierContext>()))
                .Returns(Task.FromException(new Exception("failed to send notification")));
            var flow = mocker.Create<EventHandledNotificationFlow>();
            flow.Activate();
            flow.OnNewEventHandled(new EventNotifierContext
            {
                Event = new TestEvent(),
                CurrentState = new TestState(),
                OldState = new TestState()
            });
        }

        [Fact]
        public void Activate_Deactivate()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            var flow = mocker.Create<EventHandledNotificationFlow>();
            flow.Activate();
            flow.Deactivate();
        }

        [Fact]
        public void Deactivate_WithoutActivated()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            var flow = mocker.Create<EventHandledNotificationFlow>();
            flow.Deactivate();
        }
    }
}