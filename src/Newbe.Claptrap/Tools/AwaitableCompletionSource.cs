using System.Diagnostics;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    /// <summary>
    /// 表示可重复等待的手动设置结果的任务源
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    [DebuggerDisplay("IsCompleted = {IsCompleted}")]
    public class AwaitableCompletionSource<TResult> : ICriticalNotifyCompletion
    {
        /// <summary>
        /// 完成标记委托
        /// </summary>
        private static readonly Action callbackCompleted = () => { };

        /// <summary>
        /// 延续的任务
        /// </summary>
        private Action callback;

        /// <summary>
        /// 结果值
        /// </summary>
        private TResult result;

        /// <summary>
        /// 异常值
        /// </summary>
        private Exception exception;

        /// <summary>
        /// 获取任务的结果值类型
        /// </summary>
        public Type ResultType { get; } = typeof(TResult);

        /// <summary>
        /// 获取任务是否已完成
        /// </summary>
        public bool IsCompleted => ReferenceEquals(this.callback, callbackCompleted);

        /// <summary>
        /// 获取等待对象
        /// </summary>
        /// <returns></returns>
        public AwaitableCompletionSource<TResult> GetAwaiter()
        {
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public TResult GetResult()
        {
            if (this.IsCompleted == false)
            {
                throw new InvalidOperationException();
            }

            this.callback = null;
            if (this.exception != null)
            {
                throw this.exception;
            }
            return this.result;
        }

        /// <summary>
        /// 设置任务结果
        /// </summary>
        /// <param name="result">结果值</param>
        /// <returns></returns>
        public bool SetResult(object result)
        {
            return this.SetResult((TResult)result);
        }

        /// <summary>
        /// 设置任务结果
        /// </summary>
        /// <param name="result">结果值</param>
        /// <returns></returns>
        public bool SetResult(TResult result)
        {
            return this.SetCompleted(result, exception: default);
        }

        /// <summary>
        /// 设置任务异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public bool SetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            return this.SetCompleted(result: default, exception);
        }


        /// <summary>
        /// 设置为已完成状态
        /// 只有第一次设置有效
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="exception">异常</param>
        /// <returns></returns>
        private bool SetCompleted(TResult result, Exception? exception)
        {
            if (IsCompleted)
            {
                return false;
            }

            this.result = result;
            this.exception = exception;

            var continuation = Interlocked.Exchange(ref callback, callbackCompleted);
            if (continuation != null)
            {
                ThreadPool.UnsafeQueueUserWorkItem(state => ((Action)state)(), continuation);
            }
            return true;
        }

        /// <summary>
        /// 完成通知
        /// </summary>
        /// <param name="continuation">延续的任务</param>
        void INotifyCompletion.OnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion)this).UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// 完成通知
        /// </summary>
        /// <param name="continuation">延续的任务</param>
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            if (ReferenceEquals(this.callback, callbackCompleted) ||
                  ReferenceEquals(Interlocked.CompareExchange(ref this.callback, continuation, null), callbackCompleted))
            {
                Task.Run(continuation);
            }
        }
    }
}