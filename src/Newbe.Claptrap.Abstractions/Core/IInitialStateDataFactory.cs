using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IInitialStateDataFactory
    {
        Task<IStateData> Create(IClaptrapIdentity identity);
    }
}