using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public class BatchOperatorOptions<T>
    {
        public BatchOperatorOptions()
        {
        }

        public BatchOperatorOptions(
            IBatchOptions options)
        {
            BufferCount = options.InsertManyWindowCount ?? 200;
            MaxBufferCount = options.InsertManyMaxWindowCount ?? 100;
            MinBufferCount = options.InsertManyMinWindowCount ?? 200_000;
            BufferTime = options.InsertManyWindowTimeInMilliseconds.HasValue
                ? TimeSpan.FromMilliseconds(options.InsertManyWindowTimeInMilliseconds.Value)
                : TimeSpan.FromMilliseconds(50);
        }

        public TimeSpan? BufferTime { get; set; }
        public int? BufferCount { get; set; }
        public int? MaxBufferCount { get; set; }
        public int? MinBufferCount { get; set; }

        /// <summary>
        /// Cache data func for create cache data while invoking DoManyFunc. e.g insert sql
        /// </summary>
        public Func<IReadOnlyDictionary<string, object>>? CacheDataFunc { get; set; }

        public DoMany DoManyFunc { get; set; } = null!;

        /// <summary>
        /// Name of <see cref="DoManyFunc"/> for display
        /// </summary>
        public string? DoManyFuncName { get; set; }

        public delegate Task DoMany(IEnumerable<T> entities, IReadOnlyDictionary<string, object>? cacheData);
    }
}