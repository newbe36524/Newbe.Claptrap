using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.DomainService.TransferAccountBalance
{
    [Claptrap("TransferAccountBalance", typeof(TransferAccountBalanceStateData))]
    public interface ITransferAccountBalance
        : IClaptrapGrain
    {
        [ClaptrapEvent(nameof(TransferAccountBalanceFinishedEventData), typeof(TransferAccountBalanceFinishedEventData))]
        Task<TransferResult> Transfer(string fromId, string toId, decimal balance);
    }
}