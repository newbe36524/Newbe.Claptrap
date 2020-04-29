using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public class ClaptrapDesignStoreAccessor : IClaptrapDesignStoreAccessor
    {
        private readonly ILogger<ClaptrapDesignStoreAccessor> _logger;
        private readonly IDictionary<string, IClaptrapDesign> _actorTypeCodeDic;
        private readonly IDictionary<Type, IClaptrapDesign> _dataTypeDic;

        public ClaptrapDesignStoreAccessor(
            ILogger<ClaptrapDesignStoreAccessor> logger,
            IClaptrapDesignStore claptrapDesignStore)
        {
            _logger = logger;
            var claptrapDesigns = claptrapDesignStore.ToArray();
            _actorTypeCodeDic = claptrapDesigns.ToDictionary(x => x.Identity.TypeCode);
            _dataTypeDic = claptrapDesigns.ToDictionary(x => x.ActorStateDataType);
        }

        public Type FindEventDataType(string actorTypeCode, string eventTypeCode)
        {
            var re = _actorTypeCodeDic[actorTypeCode].EventHandlerDesigns[eventTypeCode];
            return re.EventDataType;
        }


        public Type FindStateDataType(string actorTypeCode)
        {
            var stateDataType = _actorTypeCodeDic[actorTypeCode].ActorStateDataType;
            _logger.LogDebug(
                "state data type found for {actorTypeCode} {stateDataType}",
                actorTypeCode,
                stateDataType);
            return stateDataType;
        }

        public string FindActorTypeCode(Type type)
        {
            var code = _dataTypeDic[type].Identity.TypeCode;
            _logger.LogDebug(
                "state data type code found for {actorTypeCode} {stateDataType}",
                code,
                type);
            return code;
        }
    }
}