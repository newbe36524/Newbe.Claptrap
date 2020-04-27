using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateStore
{
    public interface IInitialStateDataFactory
    {
        Task<IStateData> Create(IActorIdentity identity);
    }
}