using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    public class AccountBalanceChangeEventHandler : IEventHandler
    {
        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public Task<IState> HandleEvent(IEventContext eventContext)
        {
            var accountInfo = (AccountInfo) eventContext.State.Data;
            var accountBalanceChangeEvent = (AccountBalanceChangeEvent) eventContext.Event.Data;
            accountInfo.Balance = accountBalanceChangeEvent.NewBalance;
            return Task.FromResult(eventContext.State);
        }
    }
}