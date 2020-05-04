using System;
using Newbe.Claptrap.Preview.Abstractions.Components;

namespace Newbe.Claptrap.Preview.Impl
{
    public interface IEventHandledNotificationFlow
    {
        void Activate();
        void Deactivate();
        void OnNewEventHandled(IEventHandledNotifierContext context);
    }
}