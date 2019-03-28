using System;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Claptrap.N11StateDataUpdaters
{
    public class BalanceChangeEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
