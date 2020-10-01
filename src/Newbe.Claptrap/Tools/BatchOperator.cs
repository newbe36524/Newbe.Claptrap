using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap
{
    public class BatchOperator<T> : IBatchOperator<T>
    {
        private readonly BatchOperatorOptions<T> _options;
        private readonly ILogger<BatchOperator<T>> _logger;
        private readonly IReadOnlyDictionary<string, object>? _cacheData;

        public delegate BatchOperator<T> Factory(BatchOperatorOptions<T> options);

        private readonly Channel<BatchItem> _channel;
        private Task _task;

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

            _channel = Channel.CreateUnbounded<BatchItem>();

            _task = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var items = new LinkedList<BatchItem>();
                        while (await _channel.Reader.WaitToReadAsync())
                        {
                            var last = DateTimeOffset.UtcNow;
                            var windowTime = last.Add(_options.BufferTime.Value);
                            while (
                                items.Count < _options.BufferCount
                                && windowTime > DateTimeOffset.UtcNow
                                && _channel.Reader.TryRead(out var item))
                            {
                                items.AddLast(item);
                            }

                            if (items.Any())
                            {
                                try
                                {
                                    var task = DoManyAsync(items);
                                    await task.Invoke();
                                }
                                finally
                                {
                                    items.Clear();
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "failed to run a batch");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                }
            });
        }

        private Func<Task> DoManyAsync(LinkedList<BatchItem> x)
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

        public async Task CreateTask(T input)
        {
            var batchItem = new BatchItem
            {
                Tcs = new TaskCompletionSource<int>(),
                Input = input
            };
            var valueTask = _channel.Writer.WriteAsync(batchItem);
            if (!valueTask.IsCompleted)
            {
                await valueTask;
            }

            await batchItem.Tcs.Task;
        }

        private struct BatchItem
        {
            public T Input { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }
    }
}