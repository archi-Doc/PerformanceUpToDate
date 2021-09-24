// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1000 // Keywords should be spaced correctly

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
[SimpleJob(launchCount: 3, warmupCount: 10, targetCount: 30)]
public class SyncDesignTest
{
    public object SyncObject { get; } = new();

    public int X;

    public ConcurrentQueue<int> Queue { get; } = new();

    public ConcurrentQueue<int> Queue2 { get; } = new();

    public SyncDesignTest()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
        for (var i = 0; i < 1_000; i++)
        {
            this.Queue2.Enqueue(i);
        }
    }

    [Benchmark]
    public int Lock()
    {
        lock (this.SyncObject)
        {
            var x = this.X;
        }

        return X;
    }

    [Benchmark]
    public int Concurrent_EnqueueDequeue()
    {
        this.Queue.Enqueue(this.X);
        if (this.Queue.TryDequeue(out var x))
        {
            return x;
        }
        else
        {
            return 0;
        }
    }

    [Benchmark]
    public int Concurrent_TryDequeue()
    {
        if (this.Queue2.TryDequeue(out var x))
        {
            return x;
        }
        else
        {
            return 0;
        }
    }

    [Benchmark]
    public int Interlocked_Increment()
    {
        return Interlocked.Increment(ref this.X);
    }
}
