using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Design;
using Newbe.Claptrap.Preview.Orleans;
using Orleans;

namespace Newbe.Claptrap.Preview.Impl
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
                .ToLookup(x => x.ClaptrapMasterDesign.Identity.TypeCode);
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
                            var grain = (IClaptrapMinionGrain) _grainFactory.GetGrain(design.ClaptrapBoxInterfaceType,
                                identity.Id);
                            yield return grain.MasterCall(@event);
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
                // TODO log
            }
        }
    }
}