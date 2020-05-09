namespace Newbe.Claptrap.Tests
{
    public class TestState : IState
    {
        public IClaptrapIdentity Identity { get; set; }
        public IStateData Data { get; set; }
        public long Version { get; set; }

        public void IncreaseVersion()
        {
            Version += 1;
        }
    }
}