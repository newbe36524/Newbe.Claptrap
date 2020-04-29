using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Components
{
    public interface IClaptrapComponentFactory<out T>
        where T : class, IClaptrapComponent
    {
        T Create(IClaptrapIdentity claptrapIdentity);
    }
}