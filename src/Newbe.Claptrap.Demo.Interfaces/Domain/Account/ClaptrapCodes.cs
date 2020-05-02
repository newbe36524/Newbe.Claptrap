namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    public static class ClaptrapCodes
    {
        public static class AccountCodes
        {
            private const string Prefix = "newbe.claptrap.demo.account__";
            public const string StateCode = Prefix + "state";

            public static class EventCodes
            {
                public const string AccountBalanceChanged = Prefix + "accountBalanceChanged";
            }
        }
    }
}