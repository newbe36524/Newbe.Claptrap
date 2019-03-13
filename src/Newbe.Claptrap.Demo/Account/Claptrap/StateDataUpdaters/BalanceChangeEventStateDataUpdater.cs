using System;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Account.Claptrap.StateDataUpdaters
{
    public class BalanceChangeEventStateDataUpdater
        : StateDataUpdaterBase<AccountStateData, BalanceChangeEventData>
    {
        public override void UpdateState(AccountStateData stateData, BalanceChangeEventData eventData)
        {
            Console.WriteLine($"balance from {stateData.Balance} to {eventData.Balance}");
            stateData.Balance = eventData.Balance;
        }
    }
}