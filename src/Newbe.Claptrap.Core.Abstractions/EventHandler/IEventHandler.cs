using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventHandler
{
    public interface IEventHandler : IAsyncDisposable
    {
        Task<IState> HandleEvent(IEventContext eventContext);
    }
}