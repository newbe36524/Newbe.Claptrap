using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Box
{
    public interface IClaptrapBoxFactory
    {
        IClaptrapBox Create(IClaptrapIdentity identity);
    }
}