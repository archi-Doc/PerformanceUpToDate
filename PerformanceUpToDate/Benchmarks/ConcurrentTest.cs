// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Arc.Collections;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate.Design;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class ConcurrentTest
{
    private const uint Key = 100;
    private object syncObject = new();
    private ConcurrentBag<uint> concurrentBag = new();
    private ConcurrentDictionary<uint, uint> concurrentDictionary = new();
    private Dictionary<uint, uint> dictionary = new();
    private UnorderedMap<uint, uint> unorderedMap = new();
    private UInt32Hashtable<uint> hashtable = new();

    public ConcurrentTest()
    {
        for (uint i = 0; i < 1000; i++)
        {
            this.concurrentBag.Add(i);
            this.concurrentDictionary.TryAdd(i, i);
            this.dictionary.TryAdd(i, i);
            this.unorderedMap.TryAdd(i, i);
            this.hashtable.TryAdd(i, i);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public bool ConcurrentDictionary()
    {
        this.concurrentDictionary.Remove(Key, out _);
        return this.concurrentDictionary.TryAdd(Key, Key);
    }

    [Benchmark]
    public bool Dictionary()
    {
        lock (this.syncObject)
        {
            this.dictionary.Remove(Key);
            return this.dictionary.TryAdd(Key, Key);
        }
    }

    [Benchmark]
    public bool UnorderedMap()
    {
        lock (this.syncObject)
        {
            this.unorderedMap.Remove(Key);
            return this.unorderedMap.TryAdd(Key, Key);
        }
    }
}
