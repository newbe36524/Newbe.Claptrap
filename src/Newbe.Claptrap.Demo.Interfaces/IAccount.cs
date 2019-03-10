using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.Interfaces
{
    [Claptrap("Account", typeof(AccountStateData))]
    public interface IAccount : IClaptrapGrain
    {
        [ClaptrapEvent(nameof(BalanceChangeEventData), typeof(BalanceChangeEventData))]
        Task AddBalance(decimal amount);

        [ClaptrapEvent(nameof(BalanceChangeEventData), typeof(BalanceChangeEventData))]
        Task<TransferResult> Transfer(decimal amount);

        [ClaptrapEvent(nameof(LockEventData), typeof(LockEventData))]
        Task Lock();

        Task<decimal> GetBalance();
    }
}