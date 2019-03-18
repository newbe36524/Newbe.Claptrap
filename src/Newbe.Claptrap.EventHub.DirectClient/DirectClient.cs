using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.EventHub.DirectClient
{
    public class DirectClient : IDirectClient
    {
        private readonly ConcurrentQueue<PublishEventContext> _queue
            = new ConcurrentQueue<PublishEventContext>();

        public static IDirectClient Instance { get; } = new DirectClient();

        private DirectClient()
        {
            Task.Run(PublishEvent);
        }

        private async Task PublishEvent()
        {
            while (true)
            {
                while (_queue.TryDequeue(out var context))
                {
                    await PublishEvent(context);
                }

                await Task.Delay(1000);
            }
        }

        private async Task PublishEvent(PublishEventContext context)
        {
            while (true)
            {
                try
                {
                    var grain = context.MinionGrain;
                    var @event = context.Event;
                    Task task;
                    if (context.MethodInfo == null)
                    {
                        task = grain.HandleOtherEvent(@event);
                    }
                    else
                    {
                        task = (Task) context.MethodInfo.Invoke(grain, new object[] {@event});
                    }
                    await task;
                    return;
                }
                catch (Exception e)
                {
                    // todo log
                    Console.WriteLine(e);
                    await Task.Delay(1000);
                }
            }
        }

        public Task PublishEvent(IMinionGrain grain, IEvent @event, MethodInfo methodInfo = null)
        {
            _queue.Enqueue(new PublishEventContext
            {
                Event = @event,
                MethodInfo = methodInfo,
                MinionGrain = grain,
            });
            return Task.CompletedTask;
        }

        private class PublishEventContext
        {
            public IEvent Event { get; set; }
            public IMinionGrain MinionGrain { get; set; }
            public MethodInfo MethodInfo { get; set; }
        }
    }
}