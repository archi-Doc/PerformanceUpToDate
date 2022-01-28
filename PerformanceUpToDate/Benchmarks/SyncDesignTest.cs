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
public class SyncDesignTest
{
    public object SyncObject { get; } = new();

    public int X;

    public int Y;

    public volatile int V;

    public ConcurrentQueue<int> Queue { get; } = new();

    public ConcurrentQueue<int> Queue2 { get; } = new();

    public ConcurrentStack<int> Stack { get; } = new();

    public SyncDesignTest()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
        /*for (var i = 0; i < 100_000_000; i++)
        {
            this.Queue2.Enqueue(i);
        }*/
    }

    [Benchmark]
    public int Copy()
    {
        this.Y = this.X;
        return this.Y;
    }

    [Benchmark]
    public int Lock()
    {
        lock (this.SyncObject)
        {
            this.Y = this.X;
        }

        return this.Y;
    }

    [Benchmark]
    public int VolatileWrite()
    {
        Volatile.Write(ref this.Y, this.X);
        return this.Y;
    }

    [Benchmark]
    public int RawIncrement()
    {
        this.X++;
        return this.X;
    }

    [Benchmark]
    public int VolatileIncrement()
    {
        this.V++;
        return this.V;
    }

    [Benchmark]
    public int InterlockedIncrement()
    {
        var y = Interlocked.Increment(ref this.Y);
        return y;
    }

    [Benchmark]
    public int InterlockedExchange()
    {
        var y = Interlocked.Exchange(ref this.Y, this.X);
        return y;
    }

    /*[Benchmark]
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
    public int Concurrent_PushPop()
    {
        this.Stack.Push(this.X);
        if (this.Stack.TryPop(out var x))
        {
            return x;
        }
        else
        {
            return 0;
        }
    }

    [IterationSetup(Target = "Concurrent_TryDequeue2")]
    public void SetupCuncurrent()
    {
        Console.WriteLine("setup");
        this.Queue2.Clear();
        for (var i = 0; i < 1_000_000; i++)
        {
            this.Queue2.Enqueue(i);
        }
    }

    [Benchmark]
    [InvocationCount(1_000_000)]
    public int Concurrent_TryDequeue2()
    {
        if (this.Queue2.TryDequeue(out var x))
        {
            return x;
        }
        else
        {
            lock (this.SyncObject)
            {
                var y = this.X;
                return y;
            }
        }
    }*/
}
