using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Context;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventHandler
{
    public interface IEventHandler : IAsyncDisposable
    {
        Task<IState> HandleEvent(IEventContext eventContext);
    }
}