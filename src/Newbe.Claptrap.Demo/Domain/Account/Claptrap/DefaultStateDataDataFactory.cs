using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap
{
    public class DefaultStateDataDataFactory
        : DefaultStateDataFactory<AccountStateData>
    {
        public DefaultStateDataDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
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