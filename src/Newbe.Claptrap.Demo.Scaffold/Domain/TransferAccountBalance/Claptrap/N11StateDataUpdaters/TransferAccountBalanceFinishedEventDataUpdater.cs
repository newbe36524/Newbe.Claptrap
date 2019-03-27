using System;
using StateData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
using EventData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceFinishedEventData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.TransferAccountBalance.Claptrap.N11StateDataUpdaters
{
    public class TransferAccountBalanceFinishedEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
