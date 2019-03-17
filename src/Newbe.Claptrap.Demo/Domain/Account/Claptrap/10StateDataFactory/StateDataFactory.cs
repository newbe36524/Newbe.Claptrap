using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap._10StateDataFactory
{
    public class StateDataFactory
        : StateDataFactory<AccountStateData>
    {
        public StateDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
        {
        }

        public override Task<AccountStateData> Create()
        {
            var accountStateData = new AccountStateData
            {
                Status = AccountStatus.Active,
                Balance = 10_000_000
            };
            return Task.FromResult(accountStateData);
        }
    }
}