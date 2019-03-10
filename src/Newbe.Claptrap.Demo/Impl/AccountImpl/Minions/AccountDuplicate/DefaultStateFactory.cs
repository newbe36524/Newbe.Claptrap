using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models;

namespace Newbe.Claptrap.Demo.Impl.AccountImpl.Minions.AccountDuplicate
{
    public class DefaultStateDataDataFactory
        : DefaultStateDataFactory<AccountDuplicateStateData>
    {
        public DefaultStateDataDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
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