using Newbe.Claptrap.Core;
using Newtonsoft.Json;

namespace Newbe.Claptrap
{
    public class NoChangeStateHolder : IStateHolder
    {
        public IState DeepCopy(IState source)
        {
            // TODO this is danger if exception thrown when handling event
            return source;
        }
    }
}