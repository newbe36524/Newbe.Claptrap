using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Orleans;
using C = Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [ClaptrapState(typeof(AccountStateData), C.ClaptrapCode)]
    [ClaptrapEvent(typeof(AccountBalanceChangeEventData), C.EventCodes.AccountBalanceChanged)]
    public interface IAccount : IClaptrapGrain
    {
        Task TransferIn(decimal amount, string uid);

        Task<decimal> GetBalance();
    }
}