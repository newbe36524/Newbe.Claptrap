using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Dapr.Core;

namespace Newbe.Claptrap.Dapr
{
    public class DaprMinionActivator : IMinionActivator
    {
        private readonly ILogger<DaprMinionActivator> _logger;
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IDictionary<string, IClaptrapDesign[]> _minionLookUp;

        public DaprMinionActivator(
            ILogger<DaprMinionActivator> logger,
            IActorProxyFactory actorProxyFactory,
            IClaptrapDesignStore claptrapDesignStore)
        {
            _logger = logger;
            _actorProxyFactory = actorProxyFactory;
            _minionLookUp = claptrapDesignStore.Where(x => x.ClaptrapMasterDesign != null)
                .ToLookup(x => x.ClaptrapMasterDesign!.ClaptrapTypeCode)
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
                    var actorProxy = _actorProxyFactory.Create(new ActorId(identity.Id), minionDesign.ClaptrapTypeCode);
                    yield return actorProxy.InvokeAsync(nameof(IClaptrapMinionActor.WakeAsync));
                }
            }
        }
    }
}