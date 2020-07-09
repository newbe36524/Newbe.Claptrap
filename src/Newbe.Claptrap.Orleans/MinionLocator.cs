using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.EventCenter;
using Newbe.Claptrap.Extensions;
using Orleans;

namespace Newbe.Claptrap.Orleans
{
    public class MinionLocator : IMinionLocator
    {
        private readonly IGrainFactory _grainFactory;
        private readonly GrainMinionProxy.Factory _grainMinionProxyFactory;
        private readonly ILogger<OrleansEventCenter> _logger;
        private readonly IReadOnlyDictionary<string, IClaptrapDesign> _minionDesignsDic;

        public MinionLocator(
            IClaptrapDesignStore designStore,
            IGrainFactory grainFactory,
            GrainMinionProxy.Factory grainMinionProxyFactory,
            ILogger<OrleansEventCenter> logger)
        {
            _grainFactory = grainFactory;
            _grainMinionProxyFactory = grainMinionProxyFactory;
            _logger = logger;
            _minionDesignsDic = designStore
                .Where(x => x.IsMinion())
                .ToDictionary(x => x.ClaptrapTypeCode);
        }

        public IMinionProxy CreateProxy(IClaptrapIdentity minionId)
        {
            if (_minionDesignsDic.TryGetValue(minionId.TypeCode, out var design))
            {
                try
                {
                    var grainInterfaceType = design.ClaptrapBoxInterfaceType;
                    _logger.LogTrace(
                        "try to activate claptrap box grain in type : {type}, id : {id}",
                        design.ClaptrapBoxInterfaceType,
                        minionId.Id);
                    var grain = (IClaptrapMinionGrain) _grainFactory.GetGrain(grainInterfaceType, minionId.Id);
                    _logger.LogTrace(
                        "success to activate claptrap grain box in type : {type}, id : {id}",
                        design.ClaptrapBoxInterfaceType,
                        minionId.Id);
                    var proxy = _grainMinionProxyFactory.Invoke(grain);
                    return proxy;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "something error while calling minions, {identity}", minionId);
                }
            }
            else
            {
                _logger.LogTrace(
                    "can not found minion design in design store for type code : {typeCode}",
                    minionId.TypeCode);
            }

            return EmptyMinionProxy.Instance;
        }
    }
}