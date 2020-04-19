using System;
using StateData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
using EventData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceFinishedEventData;
namespace Newbe.Claptrap.Demo.Scaffold.DomainService.TransferAccountBalance.Claptrap.N11StateDataUpdaters
{
    public class TransferAccountBalanceFinishedEventDataUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            // TODO please add your code here and remove the exception
            throw new NotImplementedException();
        }
    }
}
