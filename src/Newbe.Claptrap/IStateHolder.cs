using Newbe.Claptrap.Core;

namespace Newbe.Claptrap
{
    public interface IStateHolder
    {
        IState DeepCopy(IState source);
    }
}