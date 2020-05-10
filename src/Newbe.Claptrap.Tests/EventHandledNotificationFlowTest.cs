using System;
using System.Threading.Tasks;
using Moq;
using Newbe.Claptrap.Core.Impl;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class EventHandledNotificationFlowTest
    {
        [Test]
        public void OnNewEventHandled()
        {
            using var mocker = AutoMockHelper.Create();
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

        [Test]
        public void OnNewEventHandled_NotifierException()
        {
            using var mocker = AutoMockHelper.Create();
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

        [Test]
        public void Activate_Deactivate()
        {
            using var mocker = AutoMockHelper.Create();
            var flow = mocker.Create<EventHandledNotificationFlow>();
            flow.Activate();
            flow.Deactivate();
        }

        [Test]
        public void Deactivate_WithoutActivated()
        {
            using var mocker = AutoMockHelper.Create();
            var flow = mocker.Create<EventHandledNotificationFlow>();
            flow.Deactivate();
        }
    }
}