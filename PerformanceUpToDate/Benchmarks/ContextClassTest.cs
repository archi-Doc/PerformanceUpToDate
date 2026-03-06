// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using Arc.Collections;
using Arc.Crypto;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate.Internal;

#pragma warning disable CS0169
#pragma warning disable CS0414

namespace PerformanceUpToDate;

public class ContextClassArray
{
    private ContextClassTest? test;

    public string Name { get; set; } = string.Empty;

    public uint Id { get; set; }

    public uint[] Ids = new uint[4];

    public string[] Names = new string[4];

    public void Clear()
    {
        this.test = default;
        this.Name = string.Empty;
        this.Id = 0;
        Array.Clear(this.Ids);
        Array.Clear(this.Names);
    }
}

public class ContextClassHashtable
{
    private ContextClassTest? test;

    public string Name { get; set; } = string.Empty;

    public uint Id { get; set; }

    public UInt32Hashtable<string> Hashtable = new();

    public void Clear()
    {
        this.test = default;
        this.Name = string.Empty;
        this.Id = 0;
        this.Hashtable.Clear();
    }
}

[Config(typeof(BenchmarkConfig))]
public class ContextClassTest
{
    private readonly ObjectPool<ContextClassArray> arrayPool = new(() => new());
    private readonly ObjectPool<ContextClassHashtable> hashtablePool = new(() => new());

    public ContextClassTest()
    {
    }

    [Benchmark]
    public ContextClassArray NewContextClassArray()
        => new();

    [Benchmark]
    public ContextClassHashtable NewContextClassHashtable()
        => new();

    [Benchmark]
    public ContextClassArray PoolContextClassArray()
    {
        var tc = this.arrayPool.Rent();
        tc.Clear();
        this.arrayPool.Return(tc);
        return tc;
    }

    [Benchmark]
    public ContextClassHashtable PoolContextClassHashtable()
    {
        var tc = this.hashtablePool.Rent();
        tc.Clear();
        this.hashtablePool.Return(tc);
        return tc;
    }
}
