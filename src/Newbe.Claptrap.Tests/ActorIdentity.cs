using System;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Tests
{
    public class ActorIdentity : IActorIdentity
    {
        public ActorIdentity(string id, string typeCode)
        {
            Id = id;
            TypeCode = typeCode;
        }

        public string Id { get; }
        public string TypeCode { get; }

        public bool Equals(IActorIdentity other)
        {
            return Id == other.Id && TypeCode == other.TypeCode;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActorIdentity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TypeCode);
        }

        public static ActorIdentity Instance =>
            new ActorIdentity(Guid.NewGuid().ToString(), typeof(ActorIdentity).FullName!);
    }
}