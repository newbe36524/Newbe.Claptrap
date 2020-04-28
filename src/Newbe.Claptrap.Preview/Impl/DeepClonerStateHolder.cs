using Force.DeepCloner;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview
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