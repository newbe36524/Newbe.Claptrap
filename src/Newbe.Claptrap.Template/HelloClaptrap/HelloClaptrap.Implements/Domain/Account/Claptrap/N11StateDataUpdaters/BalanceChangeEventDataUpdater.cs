using System;
using Newbe.Claptrap;
using StateData = HelloClaptrap.Models.Domain.Account.AccountStateData;
using EventData = HelloClaptrap.Models.Domain.Account.BalanceChangeEventData;
namespace HelloClaptrap.Implements.Domain.Account.Claptrap.N11StateDataUpdaters
{
    public class BalanceChangeEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
