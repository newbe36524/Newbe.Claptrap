using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap.N11StateDataUpdaters
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