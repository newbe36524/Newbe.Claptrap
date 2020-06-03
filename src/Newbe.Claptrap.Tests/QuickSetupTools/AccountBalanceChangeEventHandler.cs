using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    public class AccountBalanceChangeEventHandler :
        NormalEventHandler<AccountInfo, AccountBalanceChangeEvent>
    {
        public override ValueTask HandleEvent(AccountInfo stateData, AccountBalanceChangeEvent eventData,
            IEventContext eventContext)
        {
            stateData.Balance = eventData.NewBalance;
            return new ValueTask();
        }
    }
}