using Newbe.Claptrap.Core;

namespace HelloClaptrap.Models.Domain.Account
{
    public class AccountDuplicateStateData : IStateData
    {
        public decimal Balance { get; set; }
    }
}