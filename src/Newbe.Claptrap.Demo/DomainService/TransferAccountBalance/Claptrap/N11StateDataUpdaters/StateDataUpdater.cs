using Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance;

namespace Newbe.Claptrap.Demo.DomainService.TransferAccountBalance.Claptrap.N11StateDataUpdaters
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