using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac
{
    public interface IInitialStateDataFactory
    {
        Task<IStateData> Create(IActorIdentity identity);
    }
}