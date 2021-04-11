using System.Threading.Tasks;

namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    public class AccountBalanceChangeEventHandler :
        NormalEventHandler<AccountState, AccountBalanceChangeEvent>
    {
        public override ValueTask HandleEvent(AccountState stateData, AccountBalanceChangeEvent eventData,
            IEventContext eventContext)
        {
            stateData.Balance += eventData.Diff;
            return ValueTask.CompletedTask;
        }
    }
}