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

        // ReSharper disable once NotAccessedField.Local
        private readonly Task _task;

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

            _channel = Channel.CreateBounded<BatchItem>(new BoundedChannelOptions(5_0000)
            {
                FullMode = BoundedChannelFullMode.Wait,
            });

            _task = Task.Factory.StartNew(ConsumeTasks, TaskCreationOptions.LongRunning).Unwrap();
        }

        private async Task ConsumeTasks()
        {
            while (true)
            {
                try
                {
                    var items = new List<BatchItem>(_options.BufferCount!.Value);
                    while (await _channel.Reader.WaitToReadAsync())
                    {
                        var last = DateTimeOffset.UtcNow;
                        var windowTime = last.Add(_options.BufferTime!.Value);
                        while (items.Count < _options.BufferCount
                               && windowTime > DateTimeOffset.UtcNow
                               && _channel.Reader.TryRead(out var item))
                        {
                            items.Add(item);
                        }

                        if (items.Any())
                        {
                            try
                            {
                                await DoManyAsync(items);
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
        }

        private async Task DoManyAsync(List<BatchItem> items)
        {
            try
            {
                var input = items.Select(x => x.Input).ToArray();
                _logger.LogTrace("there are {count} items to do in one batch.", input.Length);
                await _options.DoManyFunc.Invoke(input, _cacheData).ConfigureAwait(false);
                _logger.LogDebug("one batch done with {count} items", input.Length);

                Parallel.ForEach(items.Select(x => x.Vts), tcs => { tcs.SetResult(0); });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to run batch operation");
                Parallel.ForEach(items.Select(x => x.Vts), tcs => { tcs.SetException(e); });
            }
        }


        public async ValueTask CreateTask(T input)
        {
            var tcs = new ManualResetValueTaskSource<int>();
            var batchItem = new BatchItem
            {
                Vts = tcs,
                Input = input
            };
            var valueTask = _channel.Writer.WriteAsync(batchItem);
            if (!valueTask.IsCompleted)
            {
                await valueTask;
            }

            var finalValueTask = new ValueTask(batchItem.Vts, 0);
            if (!finalValueTask.IsCompleted)
            {
                await finalValueTask;
            }
        }

        private struct BatchItem
        {
            public T Input { get; set; }
            public ManualResetValueTaskSource<int> Vts { get; set; }
        }
    }
}