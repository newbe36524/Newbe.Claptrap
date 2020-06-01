using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.Tools
{
    public class BatchOperatorOptions<T>
    {
        public TimeSpan? BufferTime { get; set; } = TimeSpan.FromMilliseconds(20);
        public int? BufferCount { get; set; } = 1000;
        public Func<IEnumerable<T>, Task> DoManyFunc { get; set; } = null!;
    }
}