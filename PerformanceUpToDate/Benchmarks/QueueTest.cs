// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class QueueTest
{
    private readonly Queue<int> queue = new();
    private readonly Queue<string> queue2 = new();
    private readonly ConcurrentQueue<int> concurrentQueue = new();

    public QueueTest()
    {
    }

    [Benchmark]
    public int TestQueue()
    {
        this.queue.Enqueue(1);
        this.queue.Enqueue(2);
        _ = this.queue.Dequeue();
        return this.queue.Dequeue();
    }

    [Benchmark]
    public string TestQueue2()
    {
        this.queue2.Enqueue("1");
        this.queue2.Enqueue("2");
        _ = this.queue2.Dequeue();
        return this.queue2.Dequeue();
    }

    [Benchmark]
    public int TestConcurrentQueue()
    {
        this.concurrentQueue.Enqueue(1);
        this.concurrentQueue.Enqueue(2);
        _ = this.concurrentQueue.TryDequeue(out var i);
        _ = this.concurrentQueue.TryDequeue(out i);
        return i;
    }
}
