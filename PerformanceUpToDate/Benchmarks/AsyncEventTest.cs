// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate.Design;

#pragma warning disable SA1000
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class AsyncEventTest
{
    public ManualResetEventSlim Event { get; } = new(false);

    public AsyncManualResetEvent Event2 { get; } = new();

    public AsyncManualResetEvent2 Event3 { get; private set; } = default!;

    public AsyncEventTest()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
        this.Event3 = new();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        this.Event3.Dispose();
    }

    [Benchmark]
    public void ManualResetEventSlim()
    {
        Task.Run(() =>
        {
            this.Event.Set();
        });

        this.Event.Wait();
        this.Event.Reset();
        return;
    }

    [Benchmark]
    public void ManualResetEventSlim_B()
    {
        Task.Run(() =>
        {
            Thread.SpinWait(1000);
            this.Event.Set();
        });

        this.Event.Wait();
        this.Event2.Reset();
        return;
    }

    [Benchmark]
    public void AsyncManualResetEvent()
    {
        Task.Run(() =>
        {
            this.Event2.Set();
        });

        this.Event2.Task.Wait();
        this.Event2.Reset();
        return;
    }

    [Benchmark]
    public void AsyncManualResetEvent_B()
    {
        Task.Run(() =>
        {
            Thread.SpinWait(1000);
            this.Event2.Set();
        });

        this.Event2.Task.Wait();
        this.Event2.Reset();
        return;
    }

    /*[Benchmark]
    public void AsyncManualResetEvent2()
    {
        Task.Run(() =>
        {
            this.Event3.Set();
        });

        this.Event3.Task.Wait();
        this.Event3.Reset();
        return;
    }

    [Benchmark]
    public void AsyncManualResetEvent2_B()
    {
        Task.Run(() =>
        {
            Task.Yield();
            this.Event3.Set();
        });

        this.Event3.Task.Wait();
        this.Event3.Reset();
        return;
    }*/
}
