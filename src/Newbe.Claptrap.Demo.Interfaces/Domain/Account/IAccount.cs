using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Preview.Attributes;
using Newbe.Claptrap.Preview.Orleans;
using C = Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [ClaptrapState(typeof(AccountStateData), C.StateCode)]
    [ClaptrapEvent(typeof(AccountBalanceChangeEventData), C.EventCodes.AccountBalanceChanged)]
    public interface IAccount : IClaptrapGrain
    {
        Task TransferIn(decimal amount, string uid);

        Task<decimal> GetBalance();
    }
}