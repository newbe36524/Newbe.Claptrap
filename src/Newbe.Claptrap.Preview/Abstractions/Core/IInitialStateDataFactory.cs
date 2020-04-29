using System.Threading.Tasks;

namespace Newbe.Claptrap.Preview.Abstractions.Core
{
    public interface IInitialStateDataFactory
    {
        Task<IStateData> Create(IClaptrapIdentity identity);
    }
}