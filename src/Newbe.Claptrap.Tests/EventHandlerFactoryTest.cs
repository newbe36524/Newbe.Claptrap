using System;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.Metadata;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class EventHandlerFactoryTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EventHandlerFactoryTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Create()
        {
            using var mocker = AutoMock.GetStrict();
            mocker.VerifyAll = true;
            var eventContext = new EventContext(new TestEvent
            {
                EventTypeCode = "eventType"
            }, new TestState
            {
                Identity = new ActorIdentity(Guid.NewGuid().ToString(), "typeCode")
            });

            mocker.Mock<IClaptrapRegistrationAccessor>()
                .Setup(x => x.FindEventHandlerType(eventContext.State.Identity.TypeCode,
                    eventContext.Event.EventTypeCode))
                .Returns(typeof(TestHandler));

            var eventHandlerFactory = mocker.Create<EventHandlerFactory>();
            var eventHandler = eventHandlerFactory.Create(eventContext);
            eventHandler.Should().NotBeNull();
        }

        [Fact]
        public void NotFound()
        {
            using var mocker = AutoMock.GetStrict(builder => builder.AddLogging(_testOutputHelper));
            mocker.VerifyAll = true;
            var eventContext = new EventContext(new TestEvent
            {
                EventTypeCode = "eventType"
            }, new TestState
            {
                Identity = new ActorIdentity(Guid.NewGuid().ToString(), "typeCode")
            });

            mocker.Mock<IClaptrapRegistrationAccessor>()
                .Setup(x => x.FindEventHandlerType(eventContext.State.Identity.TypeCode,
                    eventContext.Event.EventTypeCode))
                .Returns(default(Type));

            var eventHandlerFactory = mocker.Create<EventHandlerFactory>();
            Assert.Throws<EventHandlerNotFoundException>(() => { eventHandlerFactory.Create(eventContext); });
        } }
}