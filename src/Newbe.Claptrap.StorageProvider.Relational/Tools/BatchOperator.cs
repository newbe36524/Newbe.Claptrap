using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.Relational.Tools
{
    public class BatchOperator<T> : IBatchOperator<T>
    {
        private readonly BatchOperatorOptions<T> _options;
        private readonly ILogger<BatchOperator<T>> _logger;
        private readonly IReadOnlyDictionary<string, object>? _cacheData;

        public delegate BatchOperator<T> Factory(BatchOperatorOptions<T> options);

        private readonly Subject<BatchItem> _subject = new Subject<BatchItem>();

        public BatchOperator(
            BatchOperatorOptions<T> options,
            ILogger<BatchOperator<T>> logger)
        {
            if (options.BufferTime == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.BufferCount == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
            _logger = logger;
            if (options.CacheDataFunc != null)
            {
                _cacheData = options.CacheDataFunc.Invoke();
            }

            _subject
                .Buffer(options.BufferTime.Value, options.BufferCount.Value)
                .Where(x => x.Count > 0)
                .Select(x => Observable.FromAsync(DoManyAsync(x)))
                .Concat()
                .Subscribe();
        }

        private Func<Task> DoManyAsync(IList<BatchItem> x)
        {
            return async () =>
            {
                try
                {
                    var inputs = x.Select(a => a.Input).ToArray();
                    _logger.LogTrace("there are {count} items to do in one batch.", inputs.Length);
                    await _options.DoManyFunc.Invoke(inputs, _cacheData).ConfigureAwait(false);
                    _logger.LogDebug("one batch done with {count} items", inputs.Length);
                    foreach (var batchItem in x)
                    {
                        batchItem.Tcs.SetResult(0);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "failed to run batch operation");
                    foreach (var batchItem in x)
                    {
                        batchItem.Tcs.SetException(e);
                    }
                }
            };
        }

        public Task CreateTask(T input)
        {
            var batchItem = new BatchItem
            {
                Tcs = new TaskCompletionSource<int>(),
                Input = input
            };
            _subject.OnNext(batchItem);
            return batchItem.Tcs.Task;
        }

        private struct BatchItem
        {
            public T Input { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }
    }
}