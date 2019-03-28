using System;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountDuplicateStateData;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Minion.AccountDuplicate.N11StateDataUpdaters
{
    public class BalanceChangeEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
