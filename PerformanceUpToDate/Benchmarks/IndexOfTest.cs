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
public class IndexOfTest
{
    private int[] source = default!;

    public IndexOfTest()
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
        this.Value = this.Size / 2;
        for (var n = 0; n < this.Size; n++)
        {
            this.source[n] = n;
        }
    }

    [Benchmark]
    public bool ArrayIndexOf() => Array.IndexOf(this.source, this.Value) >= 0;

    [Benchmark]
    public bool ForEqual()
    {
        for (var n = 0; n < this.Size; n++)
        {
            if (this.source[n] == this.Value)
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool ForComparer()
    {
        for (var n = 0; n < this.Size; n++)
        {
            if (this.Comparer.Compare(this.source[n], this.Value) == 0)
            {
                return true;
            }
        }

        return false;
    }
}
