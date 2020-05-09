using System.Diagnostics.CodeAnalysis;

namespace Newbe.Claptrap
{
    [ExcludeFromCodeCoverage]
    public class NoChangeStateHolder : IStateHolder
    {
        public delegate NoChangeStateHolder Factory(IClaptrapIdentity identity);

        public NoChangeStateHolder(IClaptrapIdentity identity)
        {
            Identity = identity;
        }

        public IState DeepCopy(IState source)
        {
            return source;
        }

        public IClaptrapIdentity Identity { get; }
    }
}