using Newbe.Claptrap.Demo.Interfaces.DomainService;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.DomainService.Claptrap
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