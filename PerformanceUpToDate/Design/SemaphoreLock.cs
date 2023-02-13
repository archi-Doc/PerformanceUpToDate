// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Threading.Tasks;

namespace System.Threading;

public class SemaphoreLock
{
    private readonly object syncObject;
    private volatile bool entered = false;
    private int waitCount;
    private TaskNode? head;
    private TaskNode? tail;

    public SemaphoreLock()
    {
        this.syncObject = this; // lock (this) is a bad practice but...
    }

    public bool IsEntered => this.entered;

    public bool Enter()
    {
        var lockTaken = false;
        var result = false;
        Task<bool>? task = null;

        try
        {
            if (this.entered)
            {
                var spinCount = 35 * 4; // SpinWait.SpinCountforSpinBeforeWait * 4
                SpinWait spinner = default;
                while (spinner.Count < spinCount)
                {
                    spinner.SpinOnce(sleep1Threshold: -1);
                    if (!this.entered)
                    {
                        break;
                    }
                }
            }

            Monitor.Enter(this.syncObject, ref lockTaken);
            this.waitCount++;

            if (this.head is not null)
            {// Async waiters.
                task = this.EnterAsync();
            }
            else
            {// No async waiters.
                while (this.entered)
                {
                    Monitor.Wait(this.syncObject);
                }

                this.entered = true;
                result = true;
            }
        }
        finally
        {
            if (lockTaken)
            {
                this.waitCount--;
                Monitor.Exit(this.syncObject);
            }
        }

        return task == null ? result : task.GetAwaiter().GetResult();
    }

    public Task<bool> EnterAsync()
    {
        lock (this.syncObject)
        {
            if (!this.entered)
            {
                this.entered = true;
                return Task.FromResult(true);
            }
            else
            {
                // Create the task
                var node = new TaskNode();

                // Add it to the linked list
                if (this.head == null)
                {
                    this.head = node;
                    this.tail = node;
                }
                else
                {
                    this.tail!.Next = node;
                    node.Prev = this.tail;
                    this.tail = node;
                }

                return node.Task;
            }
        }
    }

    public void Exit()
    {
        lock (this.syncObject)
        {
            if (!this.entered)
            {
                throw new InvalidOperationException();
            }

            if (this.waitCount > 0)
            {
                Monitor.Pulse(this.syncObject);
            }

            if (this.head is not null)
            {
                var waiterTask = this.head;
                this.RemoveAsyncWaiter(waiterTask);
                waiterTask.TrySetResult(result: true);
            }

            this.entered = false;
        }
    }

    private void RemoveAsyncWaiter(TaskNode task)
    {
        if (task.Next is not null)
        {
            task.Next.Prev = task.Prev;
        }

        if (task.Prev is not null)
        {
            task.Prev.Next = task.Next;
        }

        if (this.head == task)
        {
            this.head = task.Next;
        }

        if (this.tail == task)
        {
            this.tail = task.Prev;
        }

        task.Next = null;
        task.Prev = null;
    }

    private sealed class TaskNode : TaskCompletionSource<bool>
    {
#pragma warning disable SA1401 // Fields should be private
        internal TaskNode? Prev;
        internal TaskNode? Next;
#pragma warning restore SA1401 // Fields should be private

        internal TaskNode()
            : base((object?)null, TaskCreationOptions.RunContinuationsAsynchronously)
        {
        }
    }
}
