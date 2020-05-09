using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests
{
    public class TestHandler : IEventHandler
    {
        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public Task<IState> HandleEvent(IEventContext eventContext)
        {
            return Task.FromResult(eventContext.State);
        }
    }
}