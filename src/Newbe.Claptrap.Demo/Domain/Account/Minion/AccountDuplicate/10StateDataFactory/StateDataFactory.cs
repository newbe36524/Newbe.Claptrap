using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Minion.AccountDuplicate._10StateDataFactory
{
    public class StateDataFactory
        : StateDataFactory<AccountDuplicateStateData>
    {
        public StateDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
        {
        }

        public override Task<AccountDuplicateStateData> Create()
        {
            var accountStateData = new AccountDuplicateStateData
            {
                Balance = 0
            };
            return Task.FromResult(accountStateData);
        }
    }
}