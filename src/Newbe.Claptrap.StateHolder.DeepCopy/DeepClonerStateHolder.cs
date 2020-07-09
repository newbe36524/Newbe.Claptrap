using Force.DeepCloner;

namespace Newbe.Claptrap
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