using Force.DeepCloner;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap
{
    public class DeepClonerStateHolder : IStateHolder
    {
        public IState DeepCopy(IState source)
        {
            var deepClone = source.DeepClone();
            return deepClone;
        }
    }
}