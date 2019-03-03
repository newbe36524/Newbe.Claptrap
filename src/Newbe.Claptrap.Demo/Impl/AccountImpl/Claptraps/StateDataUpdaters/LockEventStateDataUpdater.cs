using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.StateDataUpdaters
{
    public class LockEventStateDataUpdater
        : StateDataUpdaterBase<AccountStateData, LockEventData>
    {
        public override void UpdateState(AccountStateData stateData, LockEventData eventData)
        {
            stateData.Status = AccountStatus.Locked;
        }
    }
}