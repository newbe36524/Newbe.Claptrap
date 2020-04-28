using System;

namespace Newbe.Claptrap.Preview.Core
{
    public class ActivateFailException : Exception
    {
        public IActorIdentity ActorIdentity { get; }

        public ActivateFailException(IActorIdentity actorIdentity)
            : this(CreateExceptionMessage(actorIdentity), actorIdentity)
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

        public ActivateFailException(Exception innerException, IActorIdentity actorIdentity) : base(
            CreateExceptionMessage(actorIdentity), innerException)
        {
            ActorIdentity = actorIdentity;
        }

        private static string CreateExceptionMessage(IActorIdentity actorIdentity)
        {
            return $"failed to activate actor : {actorIdentity.TypeCode} {actorIdentity.Id}";
        }
    }
}