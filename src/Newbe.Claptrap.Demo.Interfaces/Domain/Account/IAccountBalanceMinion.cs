using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Preview.Attributes;
using Newbe.Claptrap.Preview.Orleans;
using C = Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [ClaptrapMinion(C.ClaptrapCode)]
    [ClaptrapState(typeof(AccountStateData), C.MinionCodes.BalanceMinion)]
    public interface IAccountBalanceMinion : IClaptrapMinionGrain
    {
        Task<decimal> GetBalance();
    }
}