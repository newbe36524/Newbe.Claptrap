using System;
using Autofac;
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
        public static AutoMock CreateAutoMock(this IClaptrapDesign claptrapDesign,
            string id,
            IStateData stateData,
            Action<ContainerBuilder, ActorHost>? builderAction = default,
            Func<AutoMock>? autoMockFunc = default)
        {
            var claptrapIdentity = new ClaptrapIdentity(id, claptrapDesign.ClaptrapTypeCode);
            var actorTypeInformation = ActorTypeInformation.Get(claptrapDesign.ClaptrapBoxImplementationType);
            var actorHost = new ActorHost(actorTypeInformation,
                new ActorId(id),
                default,
                new NullLoggerFactory(),
                default);

            builderAction ??= (builder, host) => { builder.RegisterInstance(host).As<ActorHost>(); };
            autoMockFunc ??= () => AutoMock.GetStrict(builder => { builderAction.Invoke(builder, actorHost); });
            var mocker = autoMockFunc.Invoke();


            var commonService = mocker.Mock<IClaptrapActorCommonService>();
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


            return mocker;
        }
    }
}