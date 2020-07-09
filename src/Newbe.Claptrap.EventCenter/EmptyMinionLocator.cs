namespace Newbe.Claptrap.EventCenter
{
    public class EmptyMinionLocator : IMinionLocator
    {
        public IMinionProxy CreateProxy(IClaptrapIdentity minionId)
        {
            return EmptyMinionProxy.Instance;
        }
    }
}