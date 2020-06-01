using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    [ClaptrapMinion(Codes.Account)]
    [ClaptrapState(typeof(AccountInfo), Codes.AccountMinion)]
    public interface IAccountMinion
    {
        Task<decimal> GetBalanceAsync();
        Task ActivateAsync();
    }
}