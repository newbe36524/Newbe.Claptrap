using System;
using Autofac.Core;
using Autofac.Extras.Moq;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Newbe.Claptrap.Dapr.TestKit
{
    public static class AutoMockExtensions
    {
        /// <summary>
        /// Mock service needed in unit test to run a claptrap in unit test
        /// </summary>
        /// <param name="mocker"></param>
        /// <param name="claptrapDesign"></param>
        /// <param name="id"></param>
        /// <param name="stateData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="EventHandlerNotFoundException"></exception>
        public static void MockActor(this AutoMock mocker,
            IClaptrapDesign claptrapDesign,
            string id,
            IStateData stateData)
        {
            var claptrapIdentity = new ClaptrapIdentity(id, claptrapDesign.ClaptrapTypeCode);
            var actorTypeInformation = ActorTypeInformation.Get(claptrapDesign.ClaptrapBoxImplementationType);
            var commonService = mocker.Mock<IClaptrapActorCommonService>();
            commonService
                .Setup(x => x.ActorHost)
                .Returns(new ActorHost(actorTypeInformation,
                    new ActorId(id),
                    default,
                    new NullLoggerFactory(),
                    default));
            commonService
                .Setup(x => x.ClaptrapAccessor.Claptrap!.State.Data)
                .Returns(stateData);

            commonService
                .Setup(x => x.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(It.IsAny<IClaptrapBox>()))
                .Returns(claptrapDesign.ClaptrapTypeCode);

            commonService
                .Setup(x => x.ClaptrapTypeCodeFactory.FindEventTypeCode(It.IsAny<IClaptrapBox>(), It.IsAny<object>()))
                .Returns<IClaptrapBox, object>((_, eventDataType) =>
                {
                    foreach (var (k, v) in claptrapDesign.EventHandlerDesigns)
                    {
                        if (v.EventDataType == eventDataType.GetType())
                        {
                            return k;
                        }
                    }

                    throw new ArgumentOutOfRangeException(nameof(eventDataType), "Missing event type code");
                });

            commonService
                .Setup(x => x.ClaptrapAccessor.Claptrap!.HandleEventAsync(It.IsAny<IEvent>()))
                .Returns<IEvent>(e =>
                {
                    var eventContext = new EventContext(e, new DataState(claptrapIdentity, stateData, 0));
                    var eventEventTypeCode = eventContext.Event.EventTypeCode;
                    if (!claptrapDesign.EventHandlerDesigns.TryGetValue(eventEventTypeCode,
                        out var handlerDesign))
                    {
                        throw new EventHandlerNotFoundException(eventContext.State.Identity.TypeCode,
                            eventEventTypeCode);
                    }

                    var createMethod = typeof(AutoMock).GetMethod(nameof(mocker.Create))!;
                    var makeGenericMethod = createMethod.MakeGenericMethod(handlerDesign.EventHandlerType);
                    var handler =
                        (IEventHandler) makeGenericMethod.Invoke(mocker, new object[] {Array.Empty<Parameter>()})!;
                    return handler.HandleEvent(eventContext);
                });
        }
    }
}