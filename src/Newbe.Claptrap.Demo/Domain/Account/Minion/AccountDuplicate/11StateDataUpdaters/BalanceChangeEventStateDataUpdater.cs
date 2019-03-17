using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Minion.AccountDuplicate._11StateDataUpdaters
{
    public class BalanceChangeEventStateDataUpdater
        : StateDataUpdaterBase<AccountDuplicateStateData, BalanceChangeEventData>
    {
        public override void UpdateState(AccountDuplicateStateData stateData, BalanceChangeEventData eventData)
        {
//            Console.WriteLine($"update balance from {stateData.Balance} to {eventData.Balance} in account duplicate");
            stateData.Balance = eventData.Balance;
        }
    }
}