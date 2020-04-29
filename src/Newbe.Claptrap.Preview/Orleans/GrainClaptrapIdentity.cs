using System;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Orleans
{
    public class GrainClaptrapIdentity : IClaptrapIdentity
    {
        public GrainClaptrapIdentity(string id, string typeCode)
        {
            Id = id;
            TypeCode = typeCode;
        }


        public string Id { get; }
        public string TypeCode { get; }

        public bool Equals(IClaptrapIdentity other)
        {
            return other != null && (Id == other.Id && TypeCode == other.TypeCode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GrainClaptrapIdentity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TypeCode);
        }
    }
}