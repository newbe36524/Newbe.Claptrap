namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    public static class ClaptrapCodes
    {
        public const string ApplicationDomain = ".demo.claptrap.newbe";

        public static class AccountCodes
        {
            public const string ClaptrapCode = "account" + ApplicationDomain;
            public const string Suffix = "__" + ClaptrapCode;

            public static class EventCodes
            {
                public const string AccountBalanceChanged = "accountBalanceChanged" + Suffix;
            }

            public static class MinionCodes
            {
                private const string MinionSuffix = "__minion" + Suffix;
                public const string BalanceMinion = "balance" + MinionSuffix;
            }
        }
    }
}