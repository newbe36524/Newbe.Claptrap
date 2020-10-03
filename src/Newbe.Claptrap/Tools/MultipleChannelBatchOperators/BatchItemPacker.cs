using System;
using System.Buffers;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.MultipleChannelBatchOperators
{
    public class BatchItemPacker<T>
    {
        private readonly BatchOperatorOptions<T> _options;
        private readonly ILogger<MultipleChannelBatchOperator<T>> _logger;
        private readonly Channel<MultipleChannelBatchOperator<T>.BatchItem> _sourceChannel;
        private readonly Channel<MultipleChannelBatchOperator<T>.BatchItems> _targetChannel;
        private readonly ArrayPool<MultipleChannelBatchOperator<T>.BatchItem> _itemArrayPool;

        // ReSharper disable once NotAccessedField.Local
        private Task _task = null!;

        public BatchItemPacker(
            BatchOperatorOptions<T> options,
            ILogger<MultipleChannelBatchOperator<T>> logger,
            Channel<MultipleChannelBatchOperator<T>.BatchItem> sourceChannel,
            Channel<MultipleChannelBatchOperator<T>.BatchItems> targetChannel)
        {
            _options = options;
            _logger = logger;
            _sourceChannel = sourceChannel;
            _targetChannel = targetChannel;
            _itemArrayPool = ArrayPool<MultipleChannelBatchOperator<T>.BatchItem>.Shared;
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(PackItems, TaskCreationOptions.LongRunning).Unwrap();
        }

        private async Task PackItems()
        {
            while (true)
            {
                try
                {
                    while (await _sourceChannel.Reader.WaitToReadAsync())
                    {
                        var (item, finalCount) = Fetch();
                        if (finalCount > 0)
                        {
                            var valueTask = _targetChannel.Writer.WriteAsync(new MultipleChannelBatchOperator<T>.BatchItems
                            {
                                Items = item,
                                ItemCount = finalCount
                            });
                            if (!valueTask.IsCompleted)
                            {
                                await valueTask;
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

            (MultipleChannelBatchOperator<T>.BatchItem[] item, int finalCount) Fetch()
            {
                var threadCount = Math.Max(Environment.ProcessorCount / 3, 1);
                var arraySize = _options.BufferCount!.Value / threadCount;
                var finalItems = _itemArrayPool.Rent(_options.BufferCount!.Value);
                var span = finalItems.AsSpan();
                var hasItem = false;
                var countArray = new int[threadCount];
                var last = DateTimeOffset.UtcNow;
                var windowTime = last.Add(_options.BufferTime!.Value);
                if (threadCount > 1)
                {
                    var parallelLoopResult = Parallel.For(0, threadCount,
                        threadIndex =>
                        {
                            var currentCount = 0;
                            var startIndex = threadIndex * arraySize;
                            while (currentCount < arraySize
                                   && windowTime > DateTimeOffset.UtcNow
                                   && _sourceChannel.Reader.TryRead(out var item))
                            {
                                hasItem = true;
                                finalItems[startIndex + currentCount] = item;
                                currentCount++;
                            }

                            countArray[threadIndex] = currentCount;
                        });
                    while (!parallelLoopResult.IsCompleted)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(1));
                    }
                }
                else
                {
                    const int threadIndex = 0;
                    var currentCount = 0;
                    var startIndex = threadIndex * arraySize;
                    while (currentCount < arraySize
                           && windowTime > DateTimeOffset.UtcNow
                           && _sourceChannel.Reader.TryRead(out var item))
                    {
                        hasItem = true;
                        finalItems[startIndex + currentCount] = item;
                        currentCount++;
                    }

                    countArray[threadIndex] = currentCount;
                }


                if (!hasItem)
                {
                    return (finalItems, 0);
                }

                var lastIndex = 0;
                var finalCount = 0;
                for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
                {
                    var startIndex = threadIndex * arraySize;
                    var count = countArray[threadIndex];
                    if (count > 0)
                    {
                        var endIndex = startIndex + count;
                        finalCount += count;
                        if (threadIndex != 0)
                        {
                            span[startIndex..endIndex].CopyTo(span[lastIndex..(lastIndex + count)]);
                        }

                        lastIndex += count;
                    }
                }

                return (finalItems, finalCount);
            }
        }
    }
}