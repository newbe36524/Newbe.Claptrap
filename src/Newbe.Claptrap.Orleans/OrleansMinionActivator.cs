using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Design;
using Orleans;

namespace Newbe.Claptrap.Orleans
{
    public class OrleansMinionActivator : IMinionActivator
    {
        private readonly ILogger<OrleansMinionActivator> _logger;
        private readonly IGrainFactory _grainFactory;
        private readonly IDictionary<string, IClaptrapDesign[]> _minionLookUp;

        public OrleansMinionActivator(
            ILogger<OrleansMinionActivator> logger,
            IGrainFactory grainFactory,
            IClaptrapDesignStore claptrapDesignStore)
        {
            _logger = logger;
            _grainFactory = grainFactory;
            _minionLookUp = claptrapDesignStore.Where(x => x.ClaptrapMasterDesign != null)
                .ToLookup(x => x.ClaptrapMasterDesign.Identity.TypeCode)
                .ToDictionary(x => x.Key, x => x.ToArray());
        }

        public async Task WakeAsync(IClaptrapIdentity identity)
        {
            if (!_minionLookUp.TryGetValue(identity.TypeCode, out var minions))
            {
                _logger.LogDebug($"There is no minions for {identity}", identity);
                return;
            }

            try
            {
                await Task.WhenAll(WakeAllAsync());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to wake all minions");
            }

            IEnumerable<Task> WakeAllAsync()
            {
                foreach (var minionDesign in minions)
                {
                    var method =
                        typeof(IGrainFactory).GetMethod(nameof(IGrainFactory.GetGrain), 1,
                            new[] {typeof(string), typeof(string)});
                    var makeGenericMethod = method.MakeGenericMethod(minionDesign.ClaptrapBoxInterfaceType);
                    dynamic grain = makeGenericMethod.Invoke(_grainFactory,
                        new object[] {identity.Id, string.Empty});

                    IClaptrapMinionGrain minionGrain = grain;
                    yield return minionGrain.WakeAsync();

                    // if (grain is
                    //     IClaptrapMinionGrain minionGrain)
                    // {
                    // }
                    // else
                    // {
                    //     _logger.LogDebug("{type} is not {minionGrain}, can`t to wake it up",
                    //         minionDesign.ClaptrapBoxInterfaceType,
                    //         nameof(IClaptrapMinionGrain));
                    // }
                }
            }
        }
    }
}