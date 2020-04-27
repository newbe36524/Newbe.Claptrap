using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateStore
{
    public interface IInitialStateDataFactoryHandler
    {
        Task<IStateData> Create(IActorIdentity identity);
    }
}