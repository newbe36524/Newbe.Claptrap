using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Tests
{
    public class TestState : IState
    {
        public IActorIdentity Identity { get; set; }
        public IStateData Data { get; set; }
        public long Version { get; set; }

        public void IncreaseVersion()
        {
            Version += 1;
        }
    }
}