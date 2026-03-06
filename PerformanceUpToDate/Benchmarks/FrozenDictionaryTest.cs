// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Arc.Collections;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public readonly record struct KeyValueStruct
{
    public readonly int Key;

    public readonly FrozenDictionaryTest? Value;

    public KeyValueStruct(int key, FrozenDictionaryTest? value)
    {
        this.Key = key;
        this.Value = value;
    }
}

[Config(typeof(BenchmarkConfig))]
public class FrozenDictionaryTest
{
    private readonly int[] array = [0, -1, 100, 12, -32, 255, -65536, 456789, 2, -3];
    private readonly Dictionary<int, int> dictionary;
    private readonly FrozenDictionary<int, int> frozenDictionary;
    private readonly UnorderedMap<int, int> map;
    private readonly ConcurrentDictionary<int, int> concurrentDictionary;
    private readonly Int32Hashtable<int> int32Hashtable;
    private readonly (int, int)[] array2;
    private readonly KeyValueStruct[] keyValueArray;
    private readonly List<KeyValueStruct> list;

    public FrozenDictionaryTest()
    {
        this.dictionary = this.CreateDictionary();
        this.frozenDictionary = this.CreateFrozenDictionary();
        this.map = this.CreateUnorderedMap();
        this.concurrentDictionary = this.CreateConcurrentDictionary();
        this.int32Hashtable = this.CreateInt32Hashtable();
        this.array2 = this.CreateIntArray();
        this.keyValueArray = this.CreateKeyValueArray();
        this.list = this.CreateList();
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
    public Int32Hashtable<int> CreateInt32Hashtable()
    {
        var hashtable = new Int32Hashtable<int>();
        foreach (var x in this.array)
        {
            hashtable.TryAdd(x, x);
        }

        return hashtable;
    }

    [Benchmark]
    public (int, int)[] CreateIntArray()
    {
        var array2 = new (int, int)[this.array.Length];
        for (var i = 0; i < this.array.Length; i++)
        {
            array2[i] = (this.array[i], this.array[i]);
        }

        return array2;
    }

    [Benchmark]
    public KeyValueStruct[] CreateKeyValueArray()
    {
        var array = new KeyValueStruct[this.array.Length];
        for (var i = 0; i < this.array.Length; i++)
        {
            array[i] = new(this.array[i], default);
        }

        return array;
    }

    [Benchmark]
    public List<KeyValueStruct> CreateList()
    {
        var list = new List<KeyValueStruct>(capacity: this.array.Length);
        for (var i = 0; i < this.array.Length; i++)
        {
            list.Add(new(this.array[i], default));
        }

        return list;
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
    public int FindArray2()
    {
        var span = this.array2.AsSpan();
        var sum = 0;
        foreach (var x in this.array)
        {
            for (var i = 0; i < this.array2.Length; i++)
            {
                if (this.array2[i].Item1 == x)
                {
                    sum += this.array2[i].Item2;
                    break;
                }
            }
        }

        return sum;
    }

    [Benchmark]
    public int FindKeyValueArray()
    {
        var span = this.keyValueArray.AsSpan();
        var sum = 0;
        foreach (var x in this.array)
        {
            for (var i = 0; i < span.Length; i++)
            {
                if (span[i].Key == x)
                {
                    sum += x;
                    break;
                }
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

    [Benchmark]
    public int FindInt32Hashtable()
    {
        var sum = 0;
        foreach (var x in this.array)
        {
            if (this.int32Hashtable.TryGetValue(x, out var v))
            {
                sum += v;
            }
        }

        return sum;
    }

    [Benchmark]
    public int FindListy()
    {
        var sum = 0;
        foreach (var x in this.array)
        {
            for (var i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].Key == x)
                {
                    sum += x;
                    break;
                }
            }
        }

        return sum;
    }
}
