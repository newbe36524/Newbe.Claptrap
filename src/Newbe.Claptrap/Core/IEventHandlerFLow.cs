using System.Threading.Tasks;

namespace Newbe.Claptrap.Core
{
    public interface IEventHandlerFLow
    {
        void Activate();
        void Deactivate();
        Task OnNewEventReceived(IEvent @event);
    }
}