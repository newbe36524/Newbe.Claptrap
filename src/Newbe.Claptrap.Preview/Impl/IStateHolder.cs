using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview
{
    public interface IStateHolder
    {
        IState DeepCopy(IState source);
    }
}