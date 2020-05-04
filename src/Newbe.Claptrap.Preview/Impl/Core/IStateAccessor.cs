using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public interface IStateAccessor
    {
        public IState State { get; set; }
    }
}