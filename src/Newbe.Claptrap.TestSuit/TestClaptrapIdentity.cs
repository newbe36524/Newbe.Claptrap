using System;

namespace Newbe.Claptrap.Tests
{
    public class TestClaptrapIdentity : IClaptrapIdentity
    {
        public TestClaptrapIdentity(string id, string typeCode)
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

            return Equals((TestClaptrapIdentity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TypeCode);
        }

        public static TestClaptrapIdentity Instance =>
            new TestClaptrapIdentity(Guid.NewGuid().ToString(), "testClaptrap_state");
    }
}