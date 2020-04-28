using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview
{
    public class NoChangeStateHolder : IStateHolder
    {
        public IState DeepCopy(IState source)
        {
            return source;
        }
    }
}