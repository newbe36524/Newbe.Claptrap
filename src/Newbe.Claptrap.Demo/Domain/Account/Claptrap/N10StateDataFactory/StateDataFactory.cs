using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.Domain.Account;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap.N10StateDataFactory
{
    public class StateDataFactory
        : StateDataFactoryBase<StateData>
    {
        public StateDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
        {
        }

        public override Task<StateData> Create()
        {
            var accountStateData = new StateData
            {
                Status = AccountStatus.Active,
                Balance = 10_000_000
            };
            return Task.FromResult(accountStateData);
        }
    }
}