using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    public class StateDataTypeRegister : IStateDataTypeRegister
    {
        private readonly ILogger<StateDataTypeRegister> _logger;

        private readonly Dictionary<string, Type> _codeBaseActorStateType
            = new Dictionary<string, Type>();

        private readonly Dictionary<Type, string> _typeBaseActorStateType
            = new Dictionary<Type, string>();

        public StateDataTypeRegister(
            ILogger<StateDataTypeRegister> logger)
        {
            _logger = logger;
        }

        public Type FindStateDataType(string actorTypeCode)
        {
            if (_codeBaseActorStateType.TryGetValue(actorTypeCode, out var re))
            {
                _logger.LogDebug(
                    "state data type found for {actorTypeCode} {stateDataType}",
                    actorTypeCode,
                    re);
                return re;
            }

            var noneStateDataType = typeof(NoneStateData);
            _logger.LogDebug("state data type not found for {actorTypeCode}, {noneStateDataType} will be used",
                actorTypeCode, noneStateDataType);
            return noneStateDataType;
        }

        public string FindActorTypeCode(Type type)
        {
            if (_typeBaseActorStateType.TryGetValue(type, out var re))
            {
                _logger.LogDebug(
                    "state data type found for {actorTypeCode} {stateDataType}",
                    type,
                    re);
                return re;
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        public void RegisterStateDataType(string actorTypeCode, Type stateDataType)
        {
            if (_codeBaseActorStateType.TryGetValue(actorTypeCode, out var oldStateDataType))
            {
                _logger.LogDebug(
                    "state data type for {actorTypeCode} {oldStateDataType} will be replaced by {stateDataType}",
                    actorTypeCode,
                    oldStateDataType,
                    stateDataType);
            }
            else
            {
                _logger.LogDebug(
                    "add state data type {stateDataType} for {actorTypeCode}",
                    stateDataType,
                    actorTypeCode);
            }

            _codeBaseActorStateType[actorTypeCode] = stateDataType;
            _typeBaseActorStateType[stateDataType] = actorTypeCode;
        }
    }
}