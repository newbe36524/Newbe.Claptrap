using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Orleans;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap
{
    public partial class Account
    {
        public Task<decimal> GetBalance()
        {
            return Task.FromResult(ActorState.Balance);
        }
    }
}