namespace Newbe.Claptrap.Demo.Models
{
    [ClaptrapDisplayName("余额变更事件")]
    public class AccountBalanceChangeEventData
        : IEventData
    {
        public decimal Diff { get; set; }
    }
}