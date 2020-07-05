using System.Threading.Tasks;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    [ClaptrapEventHandler(typeof(AccountBalanceChangeEventHandler), Codes.AccountBalanceChangeEvent)]
    [ClaptrapEventHandler(typeof(EmptyEventHandler), UnitEvent.TypeCode)]
    public class AccountBalanceMinion :
        NormalClaptrapBox<AccountInfo>, IAccountBalanceMinion
    {
        public new delegate AccountBalanceMinion Factory(IClaptrapIdentity identity);

        public AccountBalanceMinion(IClaptrapIdentity identity, IClaptrapFactory claptrapFactory) : base(identity,
            claptrapFactory)
        {
        }

        public Task<decimal> GetBalanceAsync()
        {
            var re = (AccountInfo) Claptrap.State.Data;
            return Task.FromResult(re.Balance);
        }

        public Task ActivateAsync()
        {
            return Claptrap.ActivateAsync();
        }
    }
}