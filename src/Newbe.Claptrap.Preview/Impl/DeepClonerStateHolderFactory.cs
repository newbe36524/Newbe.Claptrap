using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class DeepClonerStateHolderFactory : IClaptrapComponentFactory<IStateHolder>
    {
        private readonly DeepClonerStateHolder.Factory _deepClonerStateHolderFactory;

        public DeepClonerStateHolderFactory(
            DeepClonerStateHolder.Factory deepClonerStateHolderFactory)
        {
            _deepClonerStateHolderFactory = deepClonerStateHolderFactory;
        }

        public IStateHolder Create(IClaptrapIdentity claptrapIdentity)
        {
            return _deepClonerStateHolderFactory.Invoke(claptrapIdentity);
        }
    }
}