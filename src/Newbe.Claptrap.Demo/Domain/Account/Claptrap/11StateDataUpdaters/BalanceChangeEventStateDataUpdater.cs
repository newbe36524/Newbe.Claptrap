using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap._11StateDataUpdaters
{
    public class BalanceChangeEventStateDataUpdater
        : StateDataUpdaterBase<AccountStateData, BalanceChangeEventData>
    {
        public override void UpdateState(AccountStateData stateData, BalanceChangeEventData eventData)
        {
//            Console.WriteLine($"balance from {stateData.Balance} to {eventData.Balance}");
            stateData.Balance = eventData.Balance;
        }
    }
}