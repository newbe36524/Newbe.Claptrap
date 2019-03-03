using Newbe.Claptrap.Core;

namespace Newbe.Claptrap
{
    public class DataState : IState
    {
        public DataState(IActorIdentity identity, IStateData data, ulong version)
        {
            Identity = identity;
            Data = data;
            Version = version;
        }

        public IActorIdentity Identity { get; }
        public IStateData Data { get; }
        public ulong Version { get; private set; }

        public void IncreaseVersion()
        {
            Version++;
        }
    }
}