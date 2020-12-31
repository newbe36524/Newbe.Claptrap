using System.Threading.Tasks;
using Newbe.Claptrap.Dapr.Core;
using Newbe.Claptrap.Demo.Models;
using C = Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [ClaptrapState(typeof(AccountStateData), C.ClaptrapCode)]
    [ClaptrapEvent(typeof(AccountBalanceChangeEventData), C.EventCodes.AccountBalanceChanged)]
    public interface IAccount : IClaptrapActor
    {
        Task<decimal> TransferInAsync(decimal amount);

        Task<decimal> GetBalanceAsync();
    }
}