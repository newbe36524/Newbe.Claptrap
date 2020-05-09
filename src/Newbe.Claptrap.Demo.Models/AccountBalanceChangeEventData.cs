namespace Newbe.Claptrap.Demo.Models
{
    public class AccountBalanceChangeEventData
        : IEventData
    {
        public decimal Diff { get; set; }
    }
}