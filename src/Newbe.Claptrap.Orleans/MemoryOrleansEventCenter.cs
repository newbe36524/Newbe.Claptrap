using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.EventCenter;
using Orleans;

namespace Newbe.Claptrap.Orleans
{
    public class MemoryOrleansEventCenter : IEventCenter
    {
        private readonly IGrainFactory _grainFactory;
        private readonly ILogger<MemoryOrleansEventCenter> _logger;
        private readonly ILookup<string, IClaptrapDesign> _minionDesignsLookup;

        public MemoryOrleansEventCenter(
            IClaptrapDesignStore designStore,
            IGrainFactory grainFactory,
            ILogger<MemoryOrleansEventCenter> logger)
        {
            _grainFactory = grainFactory;
            _logger = logger;
            _minionDesignsLookup = designStore
                .Where(x => x.ClaptrapMasterDesign != null)
                .ToLookup(x => x.ClaptrapMasterDesign.ClaptrapTypeCode);
        }

        public async Task SendToMinionsAsync(IClaptrapIdentity identity, IEvent @event)
        {
            if (_minionDesignsLookup.Contains(identity.TypeCode))
            {
                try
                {
                    var minionDesigns = _minionDesignsLookup[identity.TypeCode];
                    await Task.WhenAll(SendCore(minionDesigns));

                    IEnumerable<Task> SendCore(IEnumerable<IClaptrapDesign> designs)
                    {
                        foreach (var design in designs)
                        {
                            var grainInterfaceType = design.ClaptrapBoxInterfaceType;
                            _logger.LogTrace(
                                "try to activate claptrap box grain in type : {type}, id : {id}",
                                design.ClaptrapBoxInterfaceType,
                                identity.Id);
                            var grain = (IClaptrapMinionGrain) _grainFactory.GetGrain(grainInterfaceType, identity.Id);
                            _logger.LogTrace(
                                "success to activate claptrap grain box in type : {type}, id : {id}",
                                design.ClaptrapBoxInterfaceType,
                                identity.Id);
                            yield return grain.MasterEventReceivedAsync(@event);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "something error while calling minions, {identity}", identity);
                }
            }
            else
            {
                _logger.LogTrace(
                    "can not found minion design in design store for type code : {typeCode}",
                    identity.TypeCode);
            }
        }
    }
}