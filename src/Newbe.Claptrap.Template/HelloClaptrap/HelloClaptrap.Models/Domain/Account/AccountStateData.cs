using Newbe.Claptrap.Core;

namespace HelloClaptrap.Models.Domain.Account
{
    public class AccountStateData : IStateData
    {
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
    }
}