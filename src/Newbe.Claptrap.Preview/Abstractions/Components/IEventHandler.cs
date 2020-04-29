using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Components
{
    public interface IEventHandler : IAsyncDisposable
    {
        Task<IState> HandleEvent(IEventContext eventContext);
    }
}