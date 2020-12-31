using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors.Client;
using Newbe.Claptrap.Dapr.Core;
using Newbe.Claptrap.EventCenter;

namespace Newbe.Claptrap.Dapr
{
    public class RpcMinionProxy : IMinionProxy
    {
        public delegate RpcMinionProxy Factory(ActorProxy actorProxy);

        private readonly ActorProxy _actorProxy;
        private readonly IEventStringSerializer _eventStringSerializer;

        public RpcMinionProxy(
            ActorProxy actorProxy,
            IEventStringSerializer eventStringSerializer)
        {
            _actorProxy = actorProxy;
            _eventStringSerializer = eventStringSerializer;
        }

        public Task MasterEventReceivedAsync(IEnumerable<IEvent> events)
        {
            return _actorProxy.InvokeAsync(nameof(IClaptrapMinionActor.MasterEventReceivedJsonAsync),
                events.Select(_eventStringSerializer.Serialize));
        }
    }
}