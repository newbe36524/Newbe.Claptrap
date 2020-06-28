namespace Newbe.Claptrap
{
    public class MasterOrSelfIdentity : IMasterOrSelfIdentity
    {
        public MasterOrSelfIdentity(
            IClaptrapIdentity identity,
            IMasterClaptrapInfo? masterClaptrapInfo = null)
        {
            Identity = masterClaptrapInfo?.Identity ?? identity;
        }

        public IClaptrapIdentity Identity { get; }
    }
}