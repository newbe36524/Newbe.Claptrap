using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models.EventData;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.DomainService
{
    [Claptrap("TransferAccountBalance", typeof(TransferAccountBalanceStateData))]
    public interface ITransferAccountBalance
        : IClaptrapGrain
    {
        [ClaptrapEvent(nameof(TransferAccountBalanceFinishedEventData), typeof(TransferAccountBalanceFinishedEventData))]
        Task<TransferResult> Transfer(string fromId, string toId, decimal balance);
    }

    public class TransferAccountBalanceStateData : IStateData
    {
        public bool Finished { get; set; }
    }
}