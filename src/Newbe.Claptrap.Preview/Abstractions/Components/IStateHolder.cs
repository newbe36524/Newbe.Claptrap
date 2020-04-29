using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Components
{
    public interface IStateHolder : IClaptrapComponent
    {
        IState DeepCopy(IState source);
    }
}