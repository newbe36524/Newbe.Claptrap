using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public interface IEventHandlerFLow
    {
        void Activate();
        void Deactivate();
        Task OnNewEventReceived(IEvent @event);
    }
}