using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [ClaptrapState(typeof(AccountStateData))]
    [ClaptrapEvent(typeof(AccountBalanceChangeEventData))]
    public interface IAccount : IClaptrapGrain
    {
        Task TransferIn(decimal amount, string uid);

        Task<decimal> GetBalance();
    }
}