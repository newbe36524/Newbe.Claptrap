namespace Newbe.Claptrap.Core
{
    public interface IEventHandledNotificationFlow
    {
        void Activate();
        void Deactivate();
        void OnNewEventHandled(IEventNotifierContext context);
    }
}