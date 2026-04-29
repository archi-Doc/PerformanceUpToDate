// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Threading;
using Arc.Collections;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class CancellationTokenTest2
{
    private readonly CancellationTokenSource cts = new();
    private readonly CancellationTokenSource cts2 = new();
    private readonly ObjectPool<CancellationTokenSource> ctsPool = new(() => new());

    public CancellationTokenTest2()
    {
    }

    [Benchmark]
    public CancellationTokenSource CreateCts()
    {
        return new();
    }

    [Benchmark]
    public CancellationTokenSource RentAndReturnCts()
    {
        var cts = this.ctsPool.Rent();
        cts.TryReset();
        this.ctsPool.Return(cts);
        return cts;
    }

    [Benchmark]
    public CancellationToken GetToken()
    {
        return this.cts.Token;
    }

    [Benchmark]
    public CancellationTokenSource CreateAndCancelCts()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        return cts;
    }

    [Benchmark]
    public void Cancel()
    {
        try
        {
            this.cts2.Cancel();
        }
        catch
        {
        }
    }

    [Benchmark]
    public void CheckAndCancel()
    {
        if (!this.cts2.IsCancellationRequested)
        {
            this.cts2.Cancel();
        }
    }
}
