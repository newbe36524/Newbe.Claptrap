using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Context
{
    public interface IActorContext
    {
        IActorIdentity Identity { get; }
        IState State { get; }
        Task InitializeAsync();
        Task DisposeAsync();
    }
}