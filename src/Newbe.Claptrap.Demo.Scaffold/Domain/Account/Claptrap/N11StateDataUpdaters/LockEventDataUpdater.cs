using System;
using Newbe.Claptrap;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.LockEventData;
namespace Claptrap.N11StateDataUpdaters
{
    public class LockEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
