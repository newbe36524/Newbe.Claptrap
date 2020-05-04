using System.Threading.Tasks;

namespace Newbe.Claptrap.Preview.Impl
{
    public interface IStateRestorer
    {
        Task RestoreAsync();
    }
}