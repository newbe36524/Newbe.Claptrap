using System.Threading.Tasks;

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