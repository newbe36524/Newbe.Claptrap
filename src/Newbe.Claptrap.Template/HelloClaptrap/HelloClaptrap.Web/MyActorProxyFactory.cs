using Dapr.Actors;
using Dapr.Actors.Client;

namespace HelloClaptrap.Web
{
    public class MyActorProxyFactory : IActorProxyFactory
    {
        public TActorInterface CreateActorProxy<TActorInterface>(ActorId actorId, string actorType)
            where TActorInterface : Dapr.Actors.IActor
        {
            return ActorProxy.Create<TActorInterface>(actorId, actorType);
        }

        public ActorProxy Create(ActorId actorId, string actorType)
        {
            return ActorProxy.Create(actorId, actorType);
        }
    }
}