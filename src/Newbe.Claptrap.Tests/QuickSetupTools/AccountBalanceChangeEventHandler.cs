using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    public class AccountBalanceChangeEventHandler :
        NormalEventHandler<AccountState, AccountBalanceChangeEvent>
    {
        public override ValueTask HandleEvent(AccountState stateData, AccountBalanceChangeEvent eventData,
            IEventContext eventContext)
        {
            stateData.Balance += eventData.Diff;
            return new ValueTask();
        }
    }
}