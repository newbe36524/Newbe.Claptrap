using System.Threading.Tasks;
using Newbe.Claptrap.Dapr.Core;
using Newbe.Claptrap.Demo.Models;
using C = Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [ClaptrapDisplayName("同步余额")]
    [ClaptrapMinion(C.ClaptrapCode)]
    [ClaptrapState(typeof(AccountStateData), C.MinionCodes.BalanceMinion)]
    public interface IAccountBalanceMinion : IClaptrapMinionActor
    {
        Task<decimal> GetBalance();
    }
}