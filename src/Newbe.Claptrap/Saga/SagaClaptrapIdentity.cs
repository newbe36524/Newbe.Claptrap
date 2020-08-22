using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Newbe.Claptrap.Saga
{
    public class SagaClaptrapIdentity : ISagaClaptrapIdentity
    {
        public string FlowKey { get; }
        public IClaptrapIdentity MasterIdentity { get; }
        public Type UserDataType { get; }

        public SagaClaptrapIdentity(IClaptrapIdentity masterIdentity, string flowKey, Type userDataType)
        {
            FlowKey = flowKey;
            UserDataType = userDataType;
            MasterIdentity = masterIdentity;
            var re = CreateId(masterIdentity, flowKey);
            Id = re;
            TypeCode = SagaCodes.ClaptrapTypeCode;
        }

        private static string CreateId(IClaptrapIdentity masterIdentity, string flowKey)
        {
            var sourceId = $"{flowKey}_{masterIdentity.Id}_{masterIdentity.TypeCode}";
            var hashAlgorithm = HashAlgorithm.Create("MD5");
            Debug.Assert(hashAlgorithm != null, nameof(hashAlgorithm) + " != null");
            var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(sourceId));
            var hashKey = new StringBuilder();
            const int idMaxLength = 8;
            for (var i = 0; i < idMaxLength; i++)
            {
                var b = hash[i];
                hashKey.Append(b.ToString("x2"));
            }

            var re = hashKey.ToString();
            return re;
        }

        public string Id { get; }
        public string TypeCode { get; }

        public bool Equals(IClaptrapIdentity other)
        {
            return Id == other.Id && TypeCode == other.TypeCode;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SagaClaptrapIdentity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TypeCode);
        }
    }
}