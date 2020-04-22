using System;

namespace Newbe.Claptrap.Core
{
    public class ActivateFailException : Exception
    {
        public IActorIdentity ActorIdentity { get; set; }

        public ActivateFailException(IActorIdentity actorIdentity)
            : this($"failed to activate actor : {actorIdentity.TypeCode} {actorIdentity.Id}", actorIdentity)
        {
            ActorIdentity = actorIdentity;
        }

        public ActivateFailException(string message, IActorIdentity actorIdentity) : base(message)
        {
            ActorIdentity = actorIdentity;
        }

        public ActivateFailException(string message, Exception innerException, IActorIdentity actorIdentity) : base(
            message, innerException)
        {
            ActorIdentity = actorIdentity;
        }
    }
}