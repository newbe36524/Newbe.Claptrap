using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Impl.Localization;
using static Newbe.Claptrap.Preview.Impl.Localization.LK.L0004EventHandledNotificationFlow;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EventHandledNotificationFlow : IEventHandledNotificationFlow
    {
        private readonly ILogger<EventHandledNotificationFlow> _logger;
        private readonly IEventHandledNotifier _eventHandledNotifier;
        private readonly IL _l;
        private readonly Subject<IEventHandledNotifierContext> _eventHandledNotifierContextSeq;
        private IDisposable _eventHandledNotifierFlow = null!;

        public EventHandledNotificationFlow(
            ILogger<EventHandledNotificationFlow> logger,
            IEventHandledNotifier eventHandledNotifier,
            IL l)
        {
            _logger = logger;
            _eventHandledNotifier = eventHandledNotifier;
            _l = l;
            _eventHandledNotifierContextSeq = new Subject<IEventHandledNotifierContext>();
        }

        public void Activate()
        {
            _eventHandledNotifierFlow = _eventHandledNotifierContextSeq
                .Select(context => Observable.FromAsync(async () =>
                {
                    var version = context.Event.Version;
                    try
                    {
                        await _eventHandledNotifier.Notify(context);
                        _logger.LogDebug(_l[L001SuccessToNotify], version);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, _l[L002FailToNotify], version);
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

        public void OnNewEventHandled(IEventHandledNotifierContext context)
        {
            _eventHandledNotifierContextSeq.OnNext(context);
        }
    }
}