﻿// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1000 // Keywords should be spaced correctly

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class LockTest
{
    private object syncObject = new();
    private Semaphore semaphore = new(1, 1);
    private SemaphoreSlim semaphoreSlim = new(1, 1);
    private SemaphoreLock semaphoreLock = new();

    public LockTest()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public object CreateObject()
        => new();

    [Benchmark]
    public Semaphore CreateSemaphore()
        => new(1, 1);

    [Benchmark]
    public SemaphoreSlim CreateSemaphoreSlim()
        => new(1, 1);

    [Benchmark]
    public SemaphoreLock CreateSemaphoreLock()
        => new();

    [Benchmark]
    public void Lock()
    {
        lock (this.syncObject)
        {
        }
    }

    [Benchmark]
    public void MonitorEnterExit()
    {
        var lockTaken = false;
        try
        {
            Monitor.Enter(this.syncObject, ref lockTaken);
        }
        finally
        {
            if (lockTaken)
            {
                Monitor.Exit(this.syncObject);
            }
        }
    }

    [Benchmark]
    public void SemaphoreLockEnterExit()
    {
        var lockTaken = false;
        try
        {
            lockTaken = this.semaphoreLock.Enter();
        }
        finally
        {
            if (lockTaken)
            {
                this.semaphoreLock.Exit();
            }
        }
    }

    [Benchmark]
    public void SemaphoreWaitRelease()
    {
        try
        {
            this.semaphore.WaitOne();
        }
        finally
        {
            this.semaphore.Release();
        }
    }

    [Benchmark]
    public void SemaphoreSlimWaitRelease()
    {
        try
        {
            this.semaphoreSlim.Wait(); // Wait(Timeout.Infinite, CancellationToken.None);
        }
        finally
        {
            this.semaphoreSlim.Release();
        }
    }

    [Benchmark]
    public void SemaphoreSlimWaitRelease2()
    {
        var lockTaken = false;
        try
        {
            lockTaken = this.semaphoreSlim.Wait(Timeout.Infinite, CancellationToken.None);
        }
        finally
        {
            if (lockTaken)
            {
                this.semaphoreSlim.Release();
            }
        }
    }

    [Benchmark]
    public async Task SemaphoreSlimWaitAsync()
    {
        try
        {
            await this.semaphoreSlim.WaitAsync().ConfigureAwait(false); // Wait(Timeout.Infinite, CancellationToken.None);
        }
        finally
        {
            this.semaphoreSlim.Release();
        }
    }
}
