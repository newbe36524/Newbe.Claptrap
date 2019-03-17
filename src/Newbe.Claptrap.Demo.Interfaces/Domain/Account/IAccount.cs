using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [Claptrap("Account", typeof(AccountStateData))]
    public interface IAccount : IClaptrapGrain
    {
        [ClaptrapEvent(nameof(BalanceChangeEventData), typeof(BalanceChangeEventData))]
        Task TransferIn(decimal amount, string uid);

        [ClaptrapEvent(nameof(BalanceChangeEventData), typeof(BalanceChangeEventData))]
        Task<TransferResult> TransferOut(decimal amount, string uid);

        [ClaptrapEvent(nameof(LockEventData), typeof(LockEventData))]
        Task Lock();

        Task<decimal> GetBalance();
    }
}