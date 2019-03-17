using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Demo.Models.Domain.Account
{
    public class AccountStateData : IStateData
    {
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
    }
}