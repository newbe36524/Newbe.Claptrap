using System;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Account.Minion.AccountDuplicate.StateDataUpdaters
{
    public class BalanceChangeEventStateDataUpdater
        : StateDataUpdaterBase<AccountDuplicateStateData, BalanceChangeEventData>
    {
        public override void UpdateState(AccountDuplicateStateData stateData, BalanceChangeEventData eventData)
        {
            Console.WriteLine($"update balance from {stateData.Balance} to {eventData.Balance} in account duplicate");
            stateData.Balance = eventData.Balance;
        }
    }
}