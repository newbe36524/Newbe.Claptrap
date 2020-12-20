using System;
using System.Threading;

namespace Newbe.Claptrap
{
    public record ExceptionConcurrentList<T>
    {
        private static long _idCounter = 0;

        public ExceptionConcurrentList(ConcurrentList<T> concurrentList, Exception? exception = null)
        {
            ConcurrentList = concurrentList;
            Id = Interlocked.Increment(ref _idCounter);
            Exception = exception;
        }

        public long Id { get; }
        public ConcurrentList<T> ConcurrentList { get; }
        public Exception? Exception { get; set; }

        public void Deconstruct(out ConcurrentList<T> concurrentList, out Exception? exception)
        {
            concurrentList = ConcurrentList;
            exception = Exception;
        }
    }
}