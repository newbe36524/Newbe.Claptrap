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
        [ClaptrapEventMethod(nameof(BalanceChangeEventData), typeof(BalanceChangeEventData))]
        Task AddBalance(decimal amount);

        [ClaptrapEventMethod(nameof(BalanceChangeEventData), typeof(BalanceChangeEventData))]
        Task<TransferResult> Transfer(decimal amount);

        [ClaptrapEventMethod(nameof(LockEventData), typeof(LockEventData))]
        Task Lock();

        Task<decimal> GetBalance();
    }
}