using System;

namespace Newbe.Claptrap
{
    public record ClaptrapIdentity : IClaptrapIdentity
    {
        public ClaptrapIdentity(string id, string typeCode)
        {
            Id = id;
            TypeCode = typeCode;
        }

        public string Id { get; }
        public string TypeCode { get; }

        public virtual bool Equals(IClaptrapIdentity? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id == other.Id && TypeCode == other.TypeCode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TypeCode);
        }
    }
}