using System;
using Newbe.Claptrap;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountDuplicateStateData;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;
namespace Claptrap.N11StateDataUpdaters
{
    public class BalanceChangeEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
