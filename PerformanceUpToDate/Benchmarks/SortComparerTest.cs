// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class SortComparerTest
{
    private int[] source = default!;

    public SortComparerTest()
    {
    }

    [Params(10, 100, 10_000)]
    public int Size { get; set; }

    public int Value { get; set; }

    public IComparer<int> Comparer { get; private set; } = default!;

    [GlobalSetup]
    public void Setup()
    {
        this.source = new int[this.Size];
        this.Comparer = Comparer<int>.Default;
        var r = new Random();
        for (var n = 0; n < this.Size; n++)
        {
            this.source[n] = r.Next(this.Size);
        }
    }

    [Benchmark]
    public int[] ArraySort()
    {
        var array = new int[this.Size];
        Array.Copy(this.source, array, this.Size);
        Array.Sort(array);
        return array;
    }

    [Benchmark]
    public int[] ArraySortComparer()
    {
        var array = new int[this.Size];
        Array.Copy(this.source, array, this.Size);
        Array.Sort(array, this.Comparer);
        return array;
    }
}
