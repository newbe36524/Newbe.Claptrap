using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac
{
    public class ClaptrapComponentNotFoundException : Exception
    {
        public IActorIdentity ActorIdentity { get; }

        public Type ComponentType { get; }

        public ClaptrapComponentNotFoundException(IActorIdentity actorIdentity, Type componentType)
            : this($"component with type {componentType} for {actorIdentity} is not found", actorIdentity,
                componentType)
        {
        }

        public ClaptrapComponentNotFoundException(string message, IActorIdentity actorIdentity, Type componentType) :
            base(message)
        {
            ActorIdentity = actorIdentity;
            ComponentType = componentType;
        }

        public ClaptrapComponentNotFoundException(string message, Exception innerException,
            IActorIdentity actorIdentity, Type componentType) : base(message, innerException)
        {
            ActorIdentity = actorIdentity;
            ComponentType = componentType;
        }
    }
}