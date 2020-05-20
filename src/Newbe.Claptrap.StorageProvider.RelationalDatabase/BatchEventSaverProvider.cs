using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public abstract class BatchEventSaverProvider<T> : IDisposable
    {
        protected abstract Task SaveOneAsync(T entity);
        protected abstract Task SaveManyAsync(IEnumerable<T> entities);

        private readonly EventSaverOptions _eventSaverOptions;
        protected readonly Subject<SavingItem> Subject;
        private readonly IDisposable _handler;

        protected BatchEventSaverProvider(
            EventSaverOptions eventSaverOptions)
        {
            _eventSaverOptions = eventSaverOptions;
            Subject = new Subject<SavingItem>();
            _handler = CreateHandler(Subject);
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
                            await SaveOneAsync(item.Entity);
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
                            await SaveManyAsync(items.Select(a => a.Entity));
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

        protected struct SavingItem
        {
            public SavingItem(T entity)
            {
                Entity = entity;
                Tcs = new TaskCompletionSource<int>();
            }

            public T Entity { get; }
            public TaskCompletionSource<int> Tcs { get; }
        }

        public void Dispose()
        {
            Subject.Dispose();
            _handler.Dispose();
        }
    }
}