// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Threading.Tasks;

namespace System.Threading;

public class AsyncMonitor
{
    private readonly object syncObject = new();
    private volatile bool entered = false;
    private TaskNode? head;
    private TaskNode? tail;

    public AsyncMonitor()
    {
    }

    public bool IsEntered => this.entered;

    public bool Enter()
    {
        var lockTaken = false;
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

            // If there are any async waiters, for fairness we'll get in line behind
            // then by translating our synchronous wait into an asynchronous one that we
            // then block on (once we've released the lock).
            if (this.head is not null)
            {
                return this.EnterAsync().GetAwaiter().GetResult();
            }
            else
            {// There are no async waiters, so we can proceed with normal synchronous waiting.
                while (this.entered)
                {
                    Monitor.Wait(this.syncObject);
                }

                this.entered = true;
                return true;
            }
        }
        finally
        {
            if (lockTaken)
            {
                Monitor.Exit(this.syncObject);
            }
        }
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

            Monitor.Pulse(this.syncObject);

            // Now signal to any asynchronous waiters, if there are any.  While we've already
            // signaled the synchronous waiters, we still hold the lock, and thus
            // they won't have had an opportunity to acquire this yet.  So, when releasing
            // asynchronous waiters, we assume that all synchronous waiters will eventually
            // acquire the semaphore.  That could be a faulty assumption if those synchronous
            // waits are canceled, but the wait code path will handle that.
            if (this.head is not null)
            {
                // Get the next async waiter to release and queue it to be completed
                var waiterTask = this.head;
                this.RemoveAsyncWaiter(waiterTask); // ensures waiterTask.Next/Prev are null
                waiterTask.TrySetResult(result: true);
            }

            this.entered = false;
        }
    }

    /// <summary>Removes the waiter task from the linked list.</summary>
    /// <param name="task">The task to remove.</param>
    /// <returns>true if the waiter was in the list; otherwise, false.</returns>
    private bool RemoveAsyncWaiter(TaskNode task)
    {
        // Is the task in the list?  To be in the list, either it's the head or it has a predecessor that's in the list.
        bool wasInList = this.head == task || task.Prev is not null;

        // Remove it from the linked list
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

        return wasInList;
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

    /*private sealed class TaskNode : Task<bool>
    {
        internal TaskNode? Prev, Next;
        internal TaskNode() : base((object?)null, TaskCreationOptions.RunContinuationsAsynchronously) { }
    }*/
}
