using System.Diagnostics;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Orleans
{
    [DebuggerDisplay("Kind: {Kind} , Id: {Id}")]
    public class GrainActorIdentity : IActorIdentity
    {
        public GrainActorIdentity(IActorKind kind, string id)
        {
            Kind = kind;
            Id = id;
        }

        public IActorKind Kind { get; }
        public string Id { get; }

        public bool Equals(IActorIdentity other)
        {
            return Equals(Kind, other.Kind) && string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((IActorIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Kind != null ? Kind.GetHashCode() : 0) * 397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{nameof(Kind)}: {Kind}, {nameof(Id)}: {Id}";
        }
    }
}