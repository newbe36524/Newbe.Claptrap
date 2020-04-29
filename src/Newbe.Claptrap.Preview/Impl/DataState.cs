using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class DataState : IState
    {
        public DataState(IClaptrapIdentity identity, IStateData data, long version)
        {
            Identity = identity;
            Data = data;
            Version = version;
        }

        public IClaptrapIdentity Identity { get; }
        public IStateData Data { get; }
        public long Version { get; private set; }

        public void IncreaseVersion()
        {
            Version++;
        }
    }
}