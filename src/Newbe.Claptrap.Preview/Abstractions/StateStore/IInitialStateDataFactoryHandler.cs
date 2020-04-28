using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.StateStore
{
    public interface IInitialStateDataFactoryHandler
    {
        Task<IStateData> Create(IActorIdentity identity);
    }
}