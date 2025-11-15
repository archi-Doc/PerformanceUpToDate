// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Arc.Collections;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate.Design;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class ConcurrentTest2
{
    private readonly ConsoleKeyInfo enterKeyInfo = new('\r', ConsoleKey.Enter, false, false, false);

    private Lock lockObject = new();
    private Arc.Collections.CircularQueue<ConsoleKeyInfo> circularQueue = new(1024);
    private ConcurrentQueue<ConsoleKeyInfo> concurrentQueue = new();

    public ConcurrentTest2()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public bool CircularQueue()
    {
        using (this.lockObject.EnterScope())
        {
            this.circularQueue.TryEnqueue(this.enterKeyInfo);
        }

        using (this.lockObject.EnterScope())
        {
            this.circularQueue.TryEnqueue(this.enterKeyInfo);
        }

        using (this.lockObject.EnterScope())
        {
            this.circularQueue.TryDequeue(out var item);
        }

        using (this.lockObject.EnterScope())
        {
            return this.circularQueue.TryDequeue(out var item2);
        }
    }

    [Benchmark]
    public bool ConcurrentQueue()
    {
        this.concurrentQueue.Enqueue(this.enterKeyInfo);

        this.concurrentQueue.Enqueue(this.enterKeyInfo);

        this.concurrentQueue.TryDequeue(out var item);

        return this.concurrentQueue.TryDequeue(out var item2);
    }
}
