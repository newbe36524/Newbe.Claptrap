using Force.DeepCloner;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class DeepClonerStateHolder : IStateHolder
    {
        public delegate DeepClonerStateHolder Factory(IClaptrapIdentity identity);

        public IClaptrapIdentity Identity { get; }

        public DeepClonerStateHolder(IClaptrapIdentity identity)
        {
            Identity = identity;
        }

        public IState DeepCopy(IState source)
        {
            var deepClone = source.DeepClone();
            return deepClone;
        }
    }
}