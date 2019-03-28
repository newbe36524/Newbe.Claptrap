using Newbe.Claptrap.Core;

namespace HelloClaptrap.Models.DomainService.TransferAccountBalance
{
    public class TransferAccountBalanceStateData : IStateData
    {
        public bool Finished { get; set; }
    }
}