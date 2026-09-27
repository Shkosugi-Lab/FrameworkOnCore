using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkOnCore
{
    /// <summary>
    /// A delegate's BeginInvoke and EndInvoke (the asynchronous call .NET Framework made through Remoting), which
    /// .NET throws PlatformNotSupportedException for (DNN's Scheduler: every scheduled task). The converter rewrites
    /// the calls (FOC1005): the delegate runs on the thread pool, with the arguments of the call, and the callback
    /// is called when it is done; the result is the task, as the IAsyncResult it was.
    /// </summary>
    public static class AsyncDelegate
    {
        public static IAsyncResult BeginInvoke(Delegate target, object[] arguments, AsyncCallback callback, object state)
        {
            var task = Task.Factory.StartNew(_ =>
            {
                try { return target.DynamicInvoke(arguments); }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    throw;
                }
            }, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            if (callback != null) task.ContinueWith(t => callback(t), TaskScheduler.Default);
            return task;
        }

        /// <summary>The delegate's result (null for void), or its exception, as EndInvoke gave them.</summary>
        public static object EndInvoke(IAsyncResult result) => ((Task<object>)result).GetAwaiter().GetResult();
    }
}
