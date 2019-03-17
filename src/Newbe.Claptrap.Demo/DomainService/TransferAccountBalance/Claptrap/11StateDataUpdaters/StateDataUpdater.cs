using Newbe.Claptrap.Demo.Interfaces.DomainService.TransferAccountBalance;
using Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance;

namespace Newbe.Claptrap.Demo.DomainService.TransferAccountBalance.Claptrap._11StateDataUpdaters
{
    public class StateDataUpdater
        : StateDataUpdaterBase<TransferAccountBalanceStateData, TransferAccountBalanceFinishedEventData>
    {
        public override void UpdateState(
            TransferAccountBalanceStateData stateData,
            TransferAccountBalanceFinishedEventData eventData)
        {
            stateData.Finished = true;
        }
    }
}