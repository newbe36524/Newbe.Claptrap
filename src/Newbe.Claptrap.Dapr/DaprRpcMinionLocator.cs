using Dapr.Actors;
using Dapr.Actors.Client;
using Newbe.Claptrap.EventCenter;

namespace Newbe.Claptrap.Dapr
{
    public class DaprRpcMinionLocator : IMinionLocator
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DaprRpcMinionLocator(
            IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory;
        }

        public IMinionProxy CreateProxy(IClaptrapIdentity minionId)
        {
            var actorProxy = _actorProxyFactory.Create(new ActorId(minionId.Id), minionId.TypeCode);
            return new RpcMinionProxy(actorProxy);
        }
    }
}