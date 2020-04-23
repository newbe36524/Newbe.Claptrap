using Newbe.Claptrap.Core;

namespace Newbe.Claptrap
{
    public class NoChangeStateHolder : IStateHolder
    {
        public IState DeepCopy(IState source)
        {
            return source;
        }
    }
}