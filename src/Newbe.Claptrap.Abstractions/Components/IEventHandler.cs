using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IEventHandler : IAsyncDisposable
    {
        Task<IState> HandleEvent(IEventContext eventContext);
    }
}