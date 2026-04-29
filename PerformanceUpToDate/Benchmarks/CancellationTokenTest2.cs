// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Threading;
using Arc.Collections;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public class OpContext : CancellationTokenSource
{
}

[Config(typeof(BenchmarkConfig))]
public class CancellationTokenTest2
{
    private readonly CancellationTokenSource cts = new();
    private readonly CancellationTokenSource cts2 = new();
    private readonly OpContext opc = new();
    private readonly ObjectPool<CancellationTokenSource> ctsPool = new(() => new());
    private readonly CancellationToken token;
    private readonly CancellationToken token2;

    public CancellationTokenTest2()
    {
        this.token = this.cts.Token;
        this.token2 = this.opc.Token;
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

    [Benchmark]
    public OpContext? ExtractContext()
    {
        try
        {
            var cancellationToken = this.token2;
            var obj = Unsafe.As<CancellationToken, object>(ref cancellationToken);
            return obj as OpContext;
        }
        catch
        {
            return null;
        }
    }

    [Benchmark]
    public OpContext? ExtractContext2()
    {
        try
        {
            var cancellationToken = this.token2;
            var obj = Unsafe.As<CancellationToken, CancellationTokenSource>(ref cancellationToken);
            return obj as OpContext;
        }
        catch
        {
            return null;
        }
    }
}
