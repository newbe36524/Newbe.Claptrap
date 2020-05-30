namespace Newbe.Claptrap
{
    public class MasterClaptrapInfo : IMasterClaptrapInfo
    {
        public MasterClaptrapInfo(IClaptrapIdentity identity,
            IClaptrapDesign design)
        {
            Identity = identity;
            Design = design;
        }

        public IClaptrapIdentity Identity { get; }
        public IClaptrapDesign Design { get; }
    }
}