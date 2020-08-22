using System;

namespace Newbe.Claptrap.Saga
{
    public interface ISagaClaptrapIdentity : IClaptrapIdentity
    {
        public string FlowKey { get; }
        public IClaptrapIdentity MasterIdentity { get; }
        public Type UserDataType { get; }
    }
}