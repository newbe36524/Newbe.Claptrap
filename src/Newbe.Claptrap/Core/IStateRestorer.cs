using System.Threading.Tasks;

namespace Newbe.Claptrap.Core
{
    public interface IStateRestorer
    {
        Task RestoreAsync();
    }
}