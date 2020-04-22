using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Metadata;

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

        public void RegisterHandler(string actorTypeCode, string eventTypeCode, Type handlerType)
        {
            if (!_handlerDic.TryGetValue(actorTypeCode, out var handlerDic))
            {
                handlerDic = new Dictionary<string, Type>();
                _handlerDic[actorTypeCode] = handlerDic;
            }

            if (!handlerDic.TryGetValue(eventTypeCode, out var oldHandlerType))
            {
                _logger.LogDebug("there is no handler for {eventTypeCode}, add {handlerType}", eventTypeCode, handlerType);
            }
            else
            {
                _logger.LogDebug(
                    "there is a old handlerType {oldHandlerType} for {eventTypeCode}, replace with {handlerType}",
                    oldHandlerType,
                    eventTypeCode,
                    handlerType);
            }

            handlerDic[eventTypeCode] = handlerType;
        }

        public Type? FindHandlerType(string actorTypeCode, string eventTypeCode)
        {
            if (!_handlerDic.TryGetValue(actorTypeCode, out var actorHandlers))
            {
                _logger.LogError("there is no handlers for {actorTypeCode}", actorTypeCode);
                return null;
            }

            if (!actorHandlers.TryGetValue(eventTypeCode, out var handlerType))
            {
                _logger.LogError(
                    "there is no handlerType for {actorTypeCode} {eventTypeCode}",
                    actorTypeCode,
                    eventTypeCode);
                return null;
            }

            _logger.LogDebug("handlerType {handlerType} found for {actorTypeCode} {eventTypeCode}",
                handlerType,
                actorTypeCode,
                eventTypeCode);
            return handlerType;
        }
    }
}