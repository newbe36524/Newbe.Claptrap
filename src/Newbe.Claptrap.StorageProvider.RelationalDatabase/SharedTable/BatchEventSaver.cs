using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public class BatchEventSaver : IEventSaver, IDisposable
    {
        private readonly EventSaverOptions _eventSaverOptions;
        private readonly IEventSaverProvider _eventSaverProvider;
        private readonly Subject<SavingItem> _subject;
        private readonly IDisposable _handler;

        public BatchEventSaver(IClaptrapIdentity identity,
            EventSaverOptions eventSaverOptions,
            IEventSaverProvider eventSaverProvider)
        {
            _eventSaverOptions = eventSaverOptions;
            _eventSaverProvider = eventSaverProvider;
            Identity = identity;
            _subject = new Subject<SavingItem>();
            _handler = CreateHandler(_subject);
        }

        private IDisposable CreateHandler(IObservable<SavingItem> subject)
        {
            IObservable<IList<SavingItem>>? buffer = null;
            if (ValidCount() && ValidTime())
            {
                buffer = subject.Buffer(GetWindowTime(), GetWindowCount());
            }
            else if (ValidCount())
            {
                buffer = subject.Buffer(GetWindowCount());
            }
            else if (ValidTime())
            {
                buffer = subject.Buffer(GetWindowTime());
            }

            IDisposable handler;
            if (buffer == null)
            {
                handler = subject
                    .Select(item => Observable.FromAsync(async () =>
                    {
                        try
                        {
                            await _eventSaverProvider.SaveOneAsync(item.Event);
                            item.Tcs.SetResult(0);
                        }
                        catch (Exception e)
                        {
                            item.Tcs.SetException(e);
                        }
                    }))
                    .Concat()
                    .Subscribe();
            }
            else
            {
                handler = buffer
                    .Select(items => Observable.FromAsync(async () =>
                    {
                        try
                        {
                            await _eventSaverProvider.SaveManyAsync(items.Select(a => a.Event));
                            foreach (var savingItem in items)
                            {
                                savingItem.Tcs.SetResult(0);
                            }
                        }
                        catch (Exception e)
                        {
                            foreach (var savingItem in items)
                            {
                                savingItem.Tcs.SetException(e);
                            }
                        }
                    }))
                    .Concat()
                    .Subscribe();
            }

            return handler;

            bool ValidCount()
                => _eventSaverOptions.InsertManyWindowCount.HasValue
                   && _eventSaverOptions.InsertManyWindowCount > 0;

            bool ValidTime()
                => _eventSaverOptions.InsertManyWindowTimeInMilliseconds.HasValue
                   && _eventSaverOptions.InsertManyWindowTimeInMilliseconds > 0;

            TimeSpan GetWindowTime()
                => TimeSpan.FromMilliseconds(_eventSaverOptions.InsertManyWindowTimeInMilliseconds!.Value);

            int GetWindowCount()
                => _eventSaverOptions.InsertManyWindowCount!.Value;
        }

        public IClaptrapIdentity Identity { get; }

        public Task SaveEventAsync(IEvent @event)
        {
            var tcs = new TaskCompletionSource<int>();
            _subject.OnNext(new SavingItem
            {
                Event = @event,
                Tcs = tcs
            });
            return tcs.Task;
        }

        private struct SavingItem
        {
            public TaskCompletionSource<int> Tcs { get; set; }
            public IEvent Event { get; set; }
        }

        public void Dispose()
        {
            _subject.Dispose();
            _handler.Dispose();
        }
    }
}