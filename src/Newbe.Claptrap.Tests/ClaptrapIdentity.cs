using System;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapIdentity : IClaptrapIdentity
    {
        public ClaptrapIdentity(string id, string typeCode)
        {
            Id = id;
            TypeCode = typeCode;
        }

        public string Id { get; }
        public string TypeCode { get; }

        public bool Equals(IClaptrapIdentity other)
        {
            return Id == other.Id && TypeCode == other.TypeCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClaptrapIdentity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TypeCode);
        }

        public static ClaptrapIdentity Instance =>
            new ClaptrapIdentity(Guid.NewGuid().ToString(), typeof(ClaptrapIdentity).FullName!);
    }
}