using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Core
{
    public class ClaptrapKind : IClaptrapKind, IEquatable<ClaptrapKind>
    {
        public ClaptrapKind(ActorType actorType, string catalog)
        {
            ActorType = actorType;
            Catalog = catalog;
        }

        public ActorType ActorType { get; }
        public string Catalog { get; }

        public override string ToString()
        {
            return $"{nameof(ActorType)}: {ActorType}, {nameof(Catalog)}: {Catalog}";
        }

        public bool Equals(ClaptrapKind other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ActorType == other.ActorType && string.Equals(Catalog, other.Catalog);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClaptrapKind) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) ActorType * 397) ^ (Catalog != null ? Catalog.GetHashCode() : 0);
            }
        }
    }
}