namespace Newbe.Claptrap.EventCenter
{
    public interface IMinionLocator
    {
        IMinionProxy CreateProxy(IClaptrapIdentity minionId);
    }
}