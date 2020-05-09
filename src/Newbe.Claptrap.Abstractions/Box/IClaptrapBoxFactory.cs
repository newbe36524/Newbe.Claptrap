namespace Newbe.Claptrap.Box
{
    public interface IClaptrapBoxFactory
    {
        IClaptrapBox Create(IClaptrapIdentity identity);
    }
}