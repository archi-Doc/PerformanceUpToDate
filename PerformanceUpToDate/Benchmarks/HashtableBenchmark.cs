﻿// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class HashtableBenchmark
{
    private readonly uint[] array = [1, 0x74b416b3, 0x4a3ed64a, 0x700214c2, 0xb10baf28, 0x65fd6e51, 0x93ac2ff2, 0x3cc6fbed];
    private readonly uint a = 0x74b416b3;

    private readonly Arc.Crypto.UInt32Hashtable<uint> hashtable = new();
    private readonly Dictionary<uint, uint> dictionary = new();
    private readonly ConcurrentDictionary<uint, uint> concurrentDictionary = new();

    public HashtableBenchmark()
    {
        foreach (var x in array)
        {
            this.hashtable.TryAdd(x, x);
            this.dictionary.Add(x, x);
            this.concurrentDictionary.TryAdd(x, x);
        }

        this.hashtable.TryGetValue(a, out var y);
        this.dictionary.TryGetValue(a, out y);
        this.concurrentDictionary.TryGetValue(a, out y);
    }

    [Benchmark]
    public uint Hashtable()
    {
        this.hashtable.TryGetValue(a, out var y);
        return y;
    }

    [Benchmark]
    public uint Dictionary()
    {
        this.dictionary.TryGetValue(a, out var y);
        return y;
    }

    [Benchmark]
    public uint DictionaryLock()
    {
        lock (this.dictionary)
        {
            this.dictionary.TryGetValue(a, out var y);
            return y;
        }
    }

    [Benchmark]
    public uint ConcurrentDictionary()
    {
        this.concurrentDictionary.TryGetValue(a, out var y);
        return y;
    }

    [Benchmark]
    public Arc.Crypto.UInt32Hashtable<uint> CreateHashtable()
    {
        var hashtable = new Arc.Crypto.UInt32Hashtable<uint>();
        foreach (var x in array)
        {
            hashtable.TryAdd(x, x);
        }

        return hashtable;
    }

    [Benchmark]
    public ConcurrentDictionary<uint, uint> CreateConcurrentDictionary()
    {
        var dictionary = new ConcurrentDictionary<uint, uint>();
        foreach (var x in array)
        {
            dictionary.TryAdd(x, x);
        }

        return dictionary;
    }
}
