using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Autofac
{
    public class EventHandlerRegister : IEventHandlerRegister
    {
        private readonly ILogger<EventHandlerRegister> _logger;

        public EventHandlerRegister(
            ILogger<EventHandlerRegister> logger)
        {
            _logger = logger;
        }

        private readonly Dictionary<string, Dictionary<string, Type>>
            _handlerDic = new Dictionary<string, Dictionary<string, Type>>();

        public void RegisterHandler(string actorTypeCode, string eventType, Type handlerType)
        {
            if (!_handlerDic.TryGetValue(actorTypeCode, out var handlerDic))
            {
                handlerDic = new Dictionary<string, Type>();
                _handlerDic[actorTypeCode] = handlerDic;
            }

            if (!handlerDic.TryGetValue(eventType, out var oldHandlerType))
            {
                _logger.LogDebug("there is no handler for {eventType}, add {handlerType}", eventType, handlerType);
            }
            else
            {
                _logger.LogDebug(
                    "there is a old handlerType {oldHandlerType} for {eventType}, replace with {handlerType}",
                    oldHandlerType,
                    eventType,
                    handlerType);
            }

            handlerDic[eventType] = handlerType;
        }

        public Type? FindHandlerType(string actorTypeCode, string eventType)
        {
            if (!_handlerDic.TryGetValue(actorTypeCode, out var actorHandlers))
            {
                _logger.LogError("there is no handlers for {actorTypeCode}", actorTypeCode);
                return null;
            }

            if (!actorHandlers.TryGetValue(eventType, out var handlerType))
            {
                _logger.LogError(
                    "there is no handlerType for {actorTypeCode} {eventType}",
                    actorHandlers,
                    eventType);
                return null;
            }

            _logger.LogDebug("handlerType {handlerType} found for {actorTypeCode} {eventType}",
                handlerType,
                actorTypeCode,
                eventType);
            return handlerType;
        }
    }
}