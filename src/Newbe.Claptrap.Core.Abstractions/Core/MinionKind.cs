using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Core
{
    public class MinionKind : IMinionKind, IEquatable<MinionKind>
    {
        public ActorType ActorType { get; }
        public string Catalog { get; }
        public string MinionCatalog { get; }

        public MinionKind(ActorType actorType, string catalog, string minionCatalog)
        {
            ActorType = actorType;
            Catalog = catalog;
            MinionCatalog = minionCatalog;
        }


        public override string ToString()
        {
            return
                $"{nameof(ActorType)}: {ActorType}, {nameof(Catalog)}: {Catalog}, {nameof(MinionCatalog)}: {MinionCatalog}";
        }

        public bool Equals(MinionKind other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ActorType == other.ActorType && string.Equals(Catalog, other.Catalog) &&
                   string.Equals(MinionCatalog, other.MinionCatalog);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MinionKind) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) ActorType;
                hashCode = (hashCode * 397) ^ (Catalog != null ? Catalog.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MinionCatalog != null ? MinionCatalog.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}