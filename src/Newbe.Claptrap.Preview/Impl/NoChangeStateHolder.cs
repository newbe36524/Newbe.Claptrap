using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public class NoChangeStateHolder : IStateHolder
    {
        public IState DeepCopy(IState source)
        {
            return source;
        }
    }
}