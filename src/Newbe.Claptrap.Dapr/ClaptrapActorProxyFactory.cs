using System;
using Dapr.Actors;
using Dapr.Actors.Client;

namespace Newbe.Claptrap.Dapr
{
    internal class ClaptrapActorProxyFactory : IActorProxyFactory
    {
        public TActorInterface CreateActorProxy<TActorInterface>(ActorId actorId, string actorType)
            where TActorInterface : IActor
        {
            return ActorProxy.Create<TActorInterface>(actorId, actorType);
        }

        public ActorProxy Create(ActorId actorId, string actorType)
        {
            return ActorProxy.Create(actorId, actorType);
        }

        public TActorInterface CreateActorProxy<TActorInterface>(ActorId actorId, string actorType, ActorProxyOptions options = null) where TActorInterface : IActor
        {
            throw new NotImplementedException();
        }

        public object CreateActorProxy(ActorId actorId, Type actorInterfaceType, string actorType, ActorProxyOptions options = null)
        {
            throw new NotImplementedException();
        }

        public ActorProxy Create(ActorId actorId, string actorType, ActorProxyOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}