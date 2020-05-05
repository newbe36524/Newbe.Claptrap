namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    public static class ClaptrapCodes
    {
        public static class AccountCodes
        {
            public const string ClaptrapCode = "newbe.claptrap.demo.account";
            private const string Prefix = ClaptrapCode + "__";

            public static class EventCodes
            {
                public const string AccountBalanceChanged = Prefix + "accountBalanceChanged";
            }

            public static class MinionCodes
            {
                public const string BalanceMinion = Prefix + "m__balance";
            }
        }
    }
}