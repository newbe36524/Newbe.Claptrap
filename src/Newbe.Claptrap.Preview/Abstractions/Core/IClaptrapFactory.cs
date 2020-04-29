namespace Newbe.Claptrap.Preview.Abstractions.Core
{
    public interface IClaptrapFactory
    {
        IClaptrap Create(IClaptrapIdentity identity);
    }
}