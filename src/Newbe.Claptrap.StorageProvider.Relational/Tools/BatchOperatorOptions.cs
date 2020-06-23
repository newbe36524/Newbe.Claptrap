using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.Tools
{
    public class BatchOperatorOptions<T>
    {
        public TimeSpan? BufferTime { get; set; } = TimeSpan.FromMilliseconds(20);
        public int? BufferCount { get; set; } = 1000;

        /// <summary>
        /// Cache data func for create cache data while invoking DoManyFunc. e.g insert sql
        /// </summary>
        public Func<IReadOnlyDictionary<string, object>>? CacheDataFunc { get; set; } = null!;

        public DoMany DoManyFunc { get; set; } = null!;

        public delegate Task DoMany(IEnumerable<T> entities, IReadOnlyDictionary<string, object>? cacheData);
    }
}