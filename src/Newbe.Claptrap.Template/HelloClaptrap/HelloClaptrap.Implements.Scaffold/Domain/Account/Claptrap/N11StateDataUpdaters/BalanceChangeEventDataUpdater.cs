using System;
using Newbe.Claptrap;
using StateData = HelloClaptrap.Models.Domain.Account.AccountStateData;
using EventData = HelloClaptrap.Models.Domain.Account.BalanceChangeEventData;
namespace HelloClaptrap.Implements.Scaffold.Domain.Account.Claptrap.N11StateDataUpdaters
{
    public class BalanceChangeEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            // TODO please add your code here and remove the exception
            throw new NotImplementedException();
        }
    }
}
