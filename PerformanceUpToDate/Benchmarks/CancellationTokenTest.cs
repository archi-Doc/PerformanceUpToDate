// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Arc.Collections;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class CancellationTokenTest
{
    private int count;
    private CancellationTokenSource cts;
    private CancellationToken cancellationToken;
    private ObjectPool<CancellationTokenSource> ctsPool = new(() => new());

    public CancellationTokenTest()
    {
        this.cts = new();
        this.cancellationToken = cts.Token;
    }

    private async Task<int> TestTask()
    {
        return this.count++;
        // return Task.Delay(100);
    }

    private async Task<int> TestTask(CancellationToken cancellationToken)
    {
        return this.count++;
        // return Task.Delay(100, cancellationToken);
    }

    [Benchmark]
    public int Task_WaitAsync()
    {
        var task = this.TestTask().WaitAsync(this.cancellationToken);
        return task.GetHashCode();
    }

    [Benchmark]
    public int Task_WaitAsyncB()
    {
        var task = this.TestTask();
        return task.GetHashCode();
    }

    [Benchmark]
    public int Task_WaitAsyncC()
    {
        var task = this.TestTask().WaitAsync(default(CancellationToken));
        return task.GetHashCode();
    }

    [Benchmark]
    public int Task_WaitAsync2()
    {
        var task = this.TestTask().WaitAsync(TimeSpan.FromSeconds(1), this.cancellationToken);
        return task.GetHashCode();
    }

    /*[Benchmark]
    public int Task_WhenAny()
    {
        var task = Task.WhenAny(this.TestTask(), Task.Delay(1000, this.cancellationToken));
        return task.GetHashCode();
    }*/

    [Benchmark]
    public int Task_Cts()
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(this.cancellationToken);
        cts.CancelAfter(1000);
        var task = this.TestTask(cts.Token);
        return task.GetHashCode();
    }
}
