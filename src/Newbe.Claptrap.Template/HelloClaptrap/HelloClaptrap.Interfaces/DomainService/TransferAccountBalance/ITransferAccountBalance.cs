using System.Threading.Tasks;
using HelloClaptrap.Interfaces.Domain.Account;
using HelloClaptrap.Models.DomainService.TransferAccountBalance;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Orleans;

namespace HelloClaptrap.Interfaces.DomainService.TransferAccountBalance
{
    [Claptrap("TransferAccountBalance", typeof(TransferAccountBalanceStateData))]
    public interface ITransferAccountBalance
        : IClaptrapGrain
    {
        [ClaptrapEvent(nameof(TransferAccountBalanceFinishedEventData), typeof(TransferAccountBalanceFinishedEventData))]
        Task<TransferResult> Transfer(string fromId, string toId, decimal balance);
    }
}