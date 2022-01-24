// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable SA1000

namespace PerformanceUpToDate.Design;

public class AsyncManualResetEvent2 : IDisposable
{
    private ManualResetEventSlim manualEvent;
    private TaskCompletionSource tcs;
    private ThreadPoolRegistration registration;

    public AsyncManualResetEvent2()
    {
        this.manualEvent = new(false);
        this.tcs = new();
        this.registration = new(this.manualEvent.WaitHandle, TimeSpan.FromSeconds(1), this.tcs);
    }

    public Task Task => this.tcs.Task;

    public void Set() => this.manualEvent.Set();

    public void Reset() => this.manualEvent.Reset();

    public void Dispose()
    {
        this.registration.Dispose();
    }

    private sealed class ThreadPoolRegistration : IDisposable
    {
        private readonly RegisteredWaitHandle registeredWaitHandle;

        public ThreadPoolRegistration(WaitHandle handle, TimeSpan timeout, TaskCompletionSource tcs)
        {
            this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(handle, (state, timedOut) => ((TaskCompletionSource)state!).TrySetResult(), tcs, timeout, executeOnlyOnce: true);
        }

        public void Dispose() => this.registeredWaitHandle.Unregister(null);
    }
}
