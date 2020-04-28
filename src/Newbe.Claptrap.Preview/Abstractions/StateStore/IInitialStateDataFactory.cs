using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.StateStore
{
    public interface IInitialStateDataFactory
    {
        Task<IStateData> Create(IActorIdentity identity);
    }
}