using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Demo.Models
{
    public class AccountBalanceChangeEventData
        : IEventData
    {
        public decimal Diff { get; set; }
    }
}