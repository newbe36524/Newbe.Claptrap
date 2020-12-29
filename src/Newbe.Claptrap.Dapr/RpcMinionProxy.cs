using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors.Client;
using Newbe.Claptrap.Dapr.Core;
using Newbe.Claptrap.EventCenter;

namespace Newbe.Claptrap.Dapr
{
    internal class RpcMinionProxy : IMinionProxy
    {
        private readonly ActorProxy _actorProxy;

        public RpcMinionProxy(
            ActorProxy actorProxy)
        {
            _actorProxy = actorProxy;
        }

        public Task MasterEventReceivedAsync(IEnumerable<IEvent> events)
        {
            return _actorProxy.InvokeAsync(nameof(IClaptrapMinionActor.MasterEventReceivedAsync), events.Cast<DataEvent>());
        }
    }
}