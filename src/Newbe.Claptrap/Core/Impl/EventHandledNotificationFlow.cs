using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Localization;

namespace Newbe.Claptrap.Core.Impl
{
    public class EventHandledNotificationFlow : IEventHandledNotificationFlow
    {
        private readonly ILogger<EventHandledNotificationFlow> _logger;
        private readonly IEventNotifier _eventNotifier;
        private readonly IL _l;
        private readonly Subject<IEventNotifierContext> _eventHandledNotifierContextSeq;
        private IDisposable _eventHandledNotifierFlow = null!;

        public EventHandledNotificationFlow(
            ILogger<EventHandledNotificationFlow> logger,
            IEventNotifier eventNotifier,
            IL l)
        {
            _logger = logger;
            _eventNotifier = eventNotifier;
            _l = l;
            _eventHandledNotifierContextSeq = new Subject<IEventNotifierContext>();
        }

        public void Activate()
        {
            _eventHandledNotifierFlow = _eventHandledNotifierContextSeq
                .Select(context => Observable.FromAsync(async () =>
                {
                    try
                    {
                        await _eventNotifier.Notify(context);
                        _logger.LogDebug(_l[LK.success_to_notify_about_event_be_handled__event_version____version_],
                            context.Event.Version);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, _l[LK.failed_to_notify_about_event_be_handled__event_version____version_],
                            context.Event.Version);
                    }
                }))
                .Concat()
                .Subscribe();
        }

        public void Deactivate()
        {
            _eventHandledNotifierFlow?.Dispose();
            _eventHandledNotifierContextSeq?.Dispose();
        }

        public void OnNewEventHandled(IEventNotifierContext context)
        {
            _eventHandledNotifierContextSeq.OnNext(context);
        }
    }
}