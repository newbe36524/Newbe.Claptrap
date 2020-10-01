using System;

namespace Newbe.Claptrap
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
            return other != null && Id == other.Id && TypeCode == other.TypeCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ClaptrapIdentity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TypeCode);
        }

        public override string ToString()
        {
            return $"[{TypeCode} : {Id}]";
        }
    }
}