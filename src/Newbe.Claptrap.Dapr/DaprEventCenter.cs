using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.EventCenter;
using Newbe.Claptrap.Extensions;

namespace Newbe.Claptrap.Dapr
{
    public class DaprEventCenter : IEventCenter
    {
        private readonly IMinionLocator _minionLocator;
        private readonly ILogger<DaprEventCenter> _logger;
        private readonly ILookup<string, IClaptrapDesign> _minionDesignsLookup;

        public DaprEventCenter(
            IClaptrapDesignStore designStore,
            IMinionLocator minionLocator,
            ILogger<DaprEventCenter> logger)
        {
            _minionLocator = minionLocator;
            _logger = logger;
            _minionDesignsLookup = designStore
                .Where(x => x.IsMinion())
                .ToLookup(x => x.ClaptrapMasterDesign!.ClaptrapTypeCode);
        }

        public async Task SendToMinionsAsync(IClaptrapIdentity masterId, IEvent @event)
        {
            if (_minionDesignsLookup.Contains(masterId.TypeCode))
            {
                try
                {
                    var minionDesigns = _minionDesignsLookup[masterId.TypeCode];
                    await Task.WhenAll(SendCore(minionDesigns));

                    IEnumerable<Task> SendCore(IEnumerable<IClaptrapDesign> designs)
                    {
                        foreach (var design in designs)
                        {
                            var proxy = _minionLocator.CreateProxy(new ClaptrapIdentity(masterId.Id,
                                design.ClaptrapTypeCode));
                            yield return proxy.MasterEventReceivedAsync(new[] {@event});
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "something error while calling minions, {identity}", masterId);
                }
            }
            else
            {
                _logger.LogTrace(
                    "can not found minion design in design store for type code : {typeCode}",
                    masterId.TypeCode);
            }
        }
    }
}