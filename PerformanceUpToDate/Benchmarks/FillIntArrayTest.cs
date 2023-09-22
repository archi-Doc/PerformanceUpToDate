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
public class FillIntArrayTest
{
    public int[] IntArray = default!;
    public int[] SourceArray = default!;

    public FillIntArrayTest()
    {
    }

    [Params(100, 1_000_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this.IntArray = new int[this.Size];
        this.SourceArray = new int[16];
        Array.Fill(this.SourceArray, -1);
    }

    [Benchmark]
    public int[] ForLoop()
    {
        for (var n = 0; n < this.IntArray.Length; n++)
        {
            this.IntArray[n] = -1;
        }

        return this.IntArray;
    }

    [Benchmark]
    public int[] ArrayFill()
    {
        Array.Fill(this.IntArray, -1);
        return this.IntArray;
    }

    [Benchmark]
    public int[] ArrayCopy()
    {
        var position = 0;
        var block = this.SourceArray.Length;
        while ((this.IntArray.Length - position) > block)
        {
            Array.Copy(this.SourceArray, 0, this.IntArray, position, block);
            position += block;
        }

        for (var n = position; n < this.IntArray.Length; n++)
        {
            this.IntArray[n] = -1;
        }

        return this.IntArray;
    }

    [Benchmark]
    public int[] BlockCopy()
    {
        var position = 0;
        var size = this.IntArray.Length * sizeof(int);
        var block = this.SourceArray.Length * sizeof(int);
        while ((size - position) > block)
        {
            Buffer.BlockCopy(this.SourceArray, 0, this.IntArray, position, block);
            position += block;
        }

        position /= sizeof(int);
        for (var n = position; n < this.IntArray.Length; n++)
        {
            this.IntArray[n] = -1;
        }

        return this.IntArray;
    }
}
