using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Metadata
{
    public class ActorMetadataNotFoundException : Exception
    {
        public IActorKind ActorKind { get; }

        public ActorMetadataNotFoundException(IActorKind actorKind)
            : base($"actor metadata not found for actor kind : {actorKind}")
        {
            ActorKind = actorKind;
        }

        public ActorMetadataNotFoundException(string message, IActorKind actorKind) : base(message)
        {
            ActorKind = actorKind;
        }

        public ActorMetadataNotFoundException(string message, Exception innerException, IActorKind actorKind) : base(
            message, innerException)
        {
            ActorKind = actorKind;
        }
    }
}