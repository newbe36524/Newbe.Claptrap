namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    public class AccountBalanceChangeEvent : IEventData
    {
        public decimal Diff { get; set; }
    }
}