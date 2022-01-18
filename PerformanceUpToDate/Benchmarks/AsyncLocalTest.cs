// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable SA1000

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class AsyncLocalTest
{
    public class AsyncLocalClass
    {
        public AsyncLocalClass()
        {
        }
    }

    public AsyncLocal<AsyncLocalClass> AsyncLocal1 { get; }

    public AsyncLocalClass Class1 { get; } = new();

    public AsyncLocalTest()
    {
        this.AsyncLocal1 = new();
        this.AsyncLocal1.Value = new();
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public AsyncLocalClass GetAsyncLocal()
    {
        return this.AsyncLocal1.Value;
    }

    [Benchmark]
    public void SetAsyncLocal()
    {
        this.AsyncLocal1.Value = this.Class1;
    }
}
