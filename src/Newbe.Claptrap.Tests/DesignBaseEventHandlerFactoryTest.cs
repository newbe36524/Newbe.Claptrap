using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class DesignBaseEventHandlerFactoryTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DesignBaseEventHandlerFactoryTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Create()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            var actorIdentity = new TestClaptrapIdentity(Guid.NewGuid().ToString(), "typeCode");
            var testEvent = new TestEvent
            {
                EventTypeCode = "eventType"
            };
            var eventContext = new EventContext(testEvent, new TestState
            {
                Identity = actorIdentity
            });

            mocker.Mock<IClaptrapDesignStore>()
                .Setup(x => x.FindDesign(actorIdentity))
                .Returns(new ClaptrapDesign
                {
                    EventHandlerDesigns = new Dictionary<string, IClaptrapEventHandlerDesign>
                    {
                        {
                            testEvent.EventTypeCode, new ClaptrapEventHandlerDesign
                            {
                                EventHandlerType = typeof(TestHandler)
                            }
                        }
                    }
                });

            var eventHandlerFactory = mocker.Create<DesignBaseEventHandlerFactory>();
            var eventHandler = eventHandlerFactory.Create(eventContext);
            eventHandler.Should().NotBeNull();
        }

        [Fact]
        public void NotFound()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            var actorIdentity = new TestClaptrapIdentity(Guid.NewGuid().ToString(), "typeCode");
            var testEvent = new TestEvent
            {
                EventTypeCode = "eventType"
            };
            var eventContext = new EventContext(testEvent, new TestState
            {
                Identity = actorIdentity
            });

            mocker.Mock<IClaptrapDesignStore>()
                .Setup(x => x.FindDesign(actorIdentity))
                .Returns(new ClaptrapDesign
                {
                    EventHandlerDesigns = new Dictionary<string, IClaptrapEventHandlerDesign>()
                });

            var eventHandlerFactory = mocker.Create<DesignBaseEventHandlerFactory>();
            Assert.Throws<EventHandlerNotFoundException>(() => { eventHandlerFactory.Create(eventContext); });
        }
    }
}