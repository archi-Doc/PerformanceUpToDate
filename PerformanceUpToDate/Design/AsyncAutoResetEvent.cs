// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceUpToDate.Design;

public class AsyncAutoResetEvent
{
    private static readonly Task CompletedTask = Task.FromResult(true);
    private readonly Queue<TaskCompletionSource<bool>> waits = new Queue<TaskCompletionSource<bool>>();
    private bool signaled;

    public Task Task
    {
        get
        {
            lock (this.waits)
            {
                if (this.signaled)
                {
                    this.signaled = false;
                    return CompletedTask;
                }
                else
                {
                    var tcs = new TaskCompletionSource<bool>();
                    this.waits.Enqueue(tcs);
                    return tcs.Task;
                }
            }
        }
    }

    public void Set()
    {
        TaskCompletionSource<bool>? toRelease = null;
        lock (this.waits)
        {
            if (this.waits.Count > 0)
            {
                toRelease = this.waits.Dequeue();
            }
            else if (!this.signaled)
            {
                this.signaled = true;
            }
        }

        if (toRelease != null)
        {
            toRelease.SetResult(true);
        }
    }
}
