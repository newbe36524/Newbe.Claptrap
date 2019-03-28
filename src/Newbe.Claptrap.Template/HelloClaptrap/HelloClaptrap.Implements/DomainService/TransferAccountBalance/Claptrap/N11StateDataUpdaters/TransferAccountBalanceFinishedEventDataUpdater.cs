using System;
using Newbe.Claptrap;
using StateData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
using EventData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceFinishedEventData;
namespace HelloClaptrap.Implements.DomainService.TransferAccountBalance.Claptrap.N11StateDataUpdaters
{
    public class TransferAccountBalanceFinishedEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
