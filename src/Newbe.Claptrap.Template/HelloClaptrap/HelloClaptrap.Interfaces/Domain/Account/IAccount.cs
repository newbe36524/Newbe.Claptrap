using System.Threading.Tasks;
using HelloClaptrap.Models.Domain.Account;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Orleans;

namespace HelloClaptrap.Interfaces.Domain.Account
{
    [Claptrap("Account", typeof(AccountStateData))]
    [TestEventStore]
    public interface IAccount : IClaptrapGrain
    {
        [ClaptrapEvent(nameof(BalanceChangeEventData), typeof(BalanceChangeEventData))]
        Task TransferIn(decimal amount, string uid);

        [ClaptrapEvent(nameof(BalanceChangeEventData), typeof(BalanceChangeEventData))]
        Task<TransferResult> TransferOut(decimal amount, string uid);

        Task<decimal> GetBalance();
    }
}