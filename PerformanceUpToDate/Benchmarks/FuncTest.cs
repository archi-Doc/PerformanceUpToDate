// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class FuncTest
{
    private readonly int x;
    private readonly Func<int, int> func = x => x + 2;
    private readonly ConcurrentDictionary<int, int> dictionary = new();

    public FuncTest()
    {
        this.x = 10;
    }

    [Benchmark]
    public bool Test1()
    {
        dictionary.GetOrAdd(this.x, static x => x + 2);
        return dictionary.TryRemove(this.x, out _);
    }

    [Benchmark]
    public bool Test2()
    {
        dictionary.GetOrAdd(this.x, x => x + this.x);
        return dictionary.TryRemove(this.x, out _);
    }
}
