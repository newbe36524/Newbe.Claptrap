using System;
using System.Collections.Generic;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Impl;
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
            using var mocker = AutoMock.GetStrict(builder => builder.AddLogging(_testOutputHelper));
            mocker.VerifyAll = true;
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