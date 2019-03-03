using System.Threading.Tasks;
using Newbe.Claptrap.Context;

namespace Newbe.Claptrap.EventHandler
{
    public interface IEventHandler : IAsyncDisposable
    {
        Task HandleEvent(IEventContext eventContext);
    }
}