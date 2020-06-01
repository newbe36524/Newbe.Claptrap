namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    public class AccountBalanceChangeEvent : IEventData
    {
        public decimal Diff { get; set; }
        public decimal NewBalance { get; set; }
    }
}