using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.EventHub.Memory
{
    public class EventReceiveChannel : IEventReceiveChannel
    {
        private readonly IClusterClient _clusterClient;
        private readonly IMinionKind _minionKind;

        public EventReceiveChannel(
            IClusterClient clusterClient,
            IMinionKind minionKind)
        {
            _clusterClient = clusterClient;
            _minionKind = minionKind;
        }

        public Task Receive(IEvent @event)
        {
            var grainId = GrainIdHelper.GetGrainId(@event.ActorIdentity);
            var minionGrain = _clusterClient.GetGrain<IMinionGrain>(grainId);
            return minionGrain.HandleEvent(@event);
        }
    }
}