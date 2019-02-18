using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreEventServer
{
    public static class WaitHandleExtensions
    {
        /// <summary>
        /// Waits for a WaitHandle using a Task Awaiter
        /// </summary>
        /// <param name="waitHandle"></param>
        /// <param name="maxWait">Max Time to Wait</param>
        /// <returns></returns>
        public static Task WaitOneAsync(this WaitHandle waitHandle, TimeSpan maxWait)
        {
            if (waitHandle == null)
                throw new ArgumentNullException("waitHandle");

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                delegate { tcs.TrySetResult(true); }, null, (int) maxWait.TotalMilliseconds, true);
            var t = tcs.Task;
            t.ContinueWith((antecedent) => rwh.Unregister(null));
            return t;
        }

        /// <summary>
        /// Waits for a WaitHandle using a Task Awaiter
        /// </summary>
        /// <param name="waitHandle"></param>
        /// <returns></returns>
        public static Task WaitOneAsync(this WaitHandle waitHandle)
        {
            if (waitHandle == null)
                throw new ArgumentNullException("waitHandle");

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                delegate { tcs.TrySetResult(true); }, null, -1, true);
            var t = tcs.Task;
            t.ContinueWith((antecedent) => rwh.Unregister(null));
            return t;
        }
    }
}
