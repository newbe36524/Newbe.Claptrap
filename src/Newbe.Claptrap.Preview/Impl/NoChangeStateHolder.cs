using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
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