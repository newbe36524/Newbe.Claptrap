namespace Newbe.Claptrap
{
    public interface IClaptrapFactory
    {
        IClaptrap Create(IClaptrapIdentity identity);
    }
}