using System;
using System.Threading.Tasks;

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