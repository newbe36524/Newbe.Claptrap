namespace Newbe.Claptrap.Box
{
    public class NormalClaptrapBox : IClaptrapBox
    {
        public delegate NormalClaptrapBox Factory(IClaptrapIdentity identity);

        public NormalClaptrapBox(
            IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory)
        {
            Claptrap = claptrapFactory.Create(identity);
        }

        public IClaptrap Claptrap { get; }
    }

    public class NormalClaptrapBox<TStateData> : NormalClaptrapBox
        where TStateData : IStateData
    {
        public NormalClaptrapBox(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory) : base(identity,
            claptrapFactory)
        {
        }

        public TStateData StateData => (TStateData) Claptrap.State.Data;
    }
}