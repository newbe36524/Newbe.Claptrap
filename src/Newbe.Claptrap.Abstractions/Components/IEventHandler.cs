using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IEventHandler : IAsyncDisposable, IDisposable
    {
        Task<IState> HandleEvent(IEventContext eventContext);
    }
}