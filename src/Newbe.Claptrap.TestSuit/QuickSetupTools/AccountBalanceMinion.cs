using System.Threading.Tasks;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    [ClaptrapEventHandler(typeof(AccountBalanceChangeEventHandler), Codes.AccountBalanceChangeEvent)]
    [ClaptrapEventHandler(typeof(EmptyEventHandler), UnitEvent.TypeCode)]
    public class AccountBalanceMinion :
        NormalClaptrapBox<AccountState>, IAccountBalanceMinion
    {
        public new delegate AccountBalanceMinion Factory(IClaptrapIdentity identity);

        public AccountBalanceMinion(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory,
            IClaptrapAccessor claptrapAccessor) : base(identity,
            claptrapFactory,
            claptrapAccessor)
        {
        }

        public Task<decimal> GetBalanceAsync()
        {
            var re = (AccountState) Claptrap.State.Data;
            return Task.FromResult(re.Balance);
        }

        public Task ActivateAsync()
        {
            return Claptrap.ActivateAsync();
        }
    }
}