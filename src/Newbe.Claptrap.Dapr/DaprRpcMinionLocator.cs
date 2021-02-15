using Dapr.Actors;
using Dapr.Actors.Client;
using Newbe.Claptrap.EventCenter;

namespace Newbe.Claptrap.Dapr
{
    public class DaprRpcMinionLocator : IMinionLocator
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly RpcMinionProxy.Factory _factory;

        public DaprRpcMinionLocator(
            IActorProxyFactory actorProxyFactory,
            RpcMinionProxy.Factory factory)
        {
            _actorProxyFactory = actorProxyFactory;
            _factory = factory;
        }

        public IMinionProxy CreateProxy(IClaptrapIdentity minionId)
        {
            var actorProxy = _actorProxyFactory.Create(new ActorId(minionId.Id), minionId.TypeCode);
            var proxy = _factory.Invoke(actorProxy);
            return proxy;
        }
    }
}