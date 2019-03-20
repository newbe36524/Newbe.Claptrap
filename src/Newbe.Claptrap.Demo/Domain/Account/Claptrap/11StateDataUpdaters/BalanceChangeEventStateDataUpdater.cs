using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap._11StateDataUpdaters
{
    public class BalanceChangeEventStateDataUpdater
        : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
//            Console.WriteLine($"balance from {stateData.Balance} to {eventData.Balance}");
            stateData.Balance = eventData.Balance;
        }
    }
}