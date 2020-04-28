using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Context;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventHandler;

namespace Newbe.Claptrap.Tests
{
    public class ExceptionHandler : IEventHandler
    {
        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public Task<IState> HandleEvent(IEventContext eventContext)
        {
            throw new Exception();
        }
    }
}