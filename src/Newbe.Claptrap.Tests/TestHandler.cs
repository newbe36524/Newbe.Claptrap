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
            if (eventContext.State.Data is TestStateData data)
            {
                data.Counter++;
            }
            return Task.FromResult(eventContext.State);
        }
    }
}