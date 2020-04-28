using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview
{
    public class DataState : IState
    {
        public DataState(IActorIdentity identity, IStateData data, long version)
        {
            Identity = identity;
            Data = data;
            Version = version;
        }

        public IActorIdentity Identity { get; }
        public IStateData Data { get; }
        public long Version { get; private set; }
        public void IncreaseVersion()
        {
            Version++;
        }
    }
}