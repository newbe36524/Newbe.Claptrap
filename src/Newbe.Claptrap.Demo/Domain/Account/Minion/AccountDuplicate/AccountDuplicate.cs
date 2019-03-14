using System.Threading.Tasks;

namespace Newbe.Claptrap.Demo.Domain.Account.Minion.AccountDuplicate
{
    public partial class AccountDuplicate
    {
        public Task<decimal> GetBalance()
        {
            return Task.FromResult(ActorState.Balance);
        }
    }
}