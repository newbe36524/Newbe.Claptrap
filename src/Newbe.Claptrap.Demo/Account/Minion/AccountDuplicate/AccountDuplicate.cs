using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.Account.Minion.AccountDuplicate
{
    public partial class AccountDuplicate
        : Grain, IAccountDuplicate
    {
        public Task<decimal> GetBalance()
        {
            return Task.FromResult(ActorState.Balance);
        }
    }
}