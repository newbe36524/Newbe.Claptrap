namespace Newbe.Claptrap
{
    public interface IClaptrapBoxFactory
    {
        IClaptrapBox Create(IClaptrapIdentity identity);
    }
}