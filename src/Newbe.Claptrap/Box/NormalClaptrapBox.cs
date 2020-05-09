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
}