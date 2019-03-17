using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Demo.Models.Domain.Account
{
    public class BalanceChangeEventData : IEventData
    {
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}