// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Arc.Collections;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class FrozenDictionaryTest
{
    private readonly int[] array = [0, -1, 100, 12, -32, 255, -65536, 456789, 2, -3];
    private readonly Dictionary<int, int> dictionary;
    private readonly FrozenDictionary<int, int> frozenDictionary;
    private readonly UnorderedMap<int, int> map;
    private readonly ConcurrentDictionary<int, int> concurrentDictionary;

    public FrozenDictionaryTest()
    {
        this.dictionary = this.CreateDictionary();
        this.frozenDictionary = this.CreateFrozenDictionary();
        this.map = this.CreateUnorderedMap();
        this.concurrentDictionary = this.CreateConcurrentDictionary();
    }

    [Benchmark]
    public Dictionary<int, int> CreateDictionary()
    {
        return this.array.ToDictionary(x => x, x => x);
    }

    [Benchmark]
    public FrozenDictionary<int, int> CreateFrozenDictionary()
    {
        return this.array.ToFrozenDictionary(x => x, x => x);
    }

    [Benchmark]
    public UnorderedMap<int, int> CreateUnorderedMap()
    {
        var map = new UnorderedMap<int, int>(this.array.Length);
        foreach (var x in this.array)
        {
            map.Add(x, x);
        }

        return map;
    }

    [Benchmark]
    public ConcurrentDictionary<int, int> CreateConcurrentDictionary()
    {
        var concurrentDictionary = new ConcurrentDictionary<int, int>();
        foreach (var x in this.array)
        {
            concurrentDictionary.TryAdd(x, x);
        }

        return concurrentDictionary;
    }

    [Benchmark]
    public int FindArray()
    {
        var span = this.array.AsSpan();
        var sum = 0;
        foreach (var x in this.array)
        {
            var idx = span.IndexOf(x);
            if (idx >= 0)
            {
                sum += span[idx];
            }
        }

        return sum;
    }

    [Benchmark]
    public int FindDictionary()
    {
        var sum = 0;
        foreach (var x in this.array)
        {
            sum += this.dictionary[x];
        }

        return sum;
    }

    [Benchmark]
    public int FindFrozenDictionary()
    {
        var sum = 0;
        foreach (var x in this.array)
        {
            sum += this.frozenDictionary[x];
        }

        return sum;
    }

    [Benchmark]
    public int FindUnorderedMap()
    {
        var sum = 0;
        foreach (var x in this.array)
        {
            sum += this.map[x];
        }

        return sum;
    }

    [Benchmark]
    public int FindConcurrentDictionary()
    {
        var sum = 0;
        foreach (var x in this.array)
        {
            sum += this.concurrentDictionary[x];
        }

        return sum;
    }
}
