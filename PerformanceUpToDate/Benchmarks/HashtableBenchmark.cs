// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

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

    public HashtableBenchmark()
    {
        foreach (var x in array)
        {
            this.hashtable.TryAdd(x, x);
            this.dictionary.Add(x, x);
        }

        this.hashtable.TryGetValue(a, out var y);
        this.dictionary.TryGetValue(a, out y);
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
}
