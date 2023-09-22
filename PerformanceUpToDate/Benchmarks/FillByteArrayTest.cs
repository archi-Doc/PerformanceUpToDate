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
public class FillByteArrayTest
{
    public byte[] ByteArray = default!;

    public FillByteArrayTest()
    {
    }

    [Params(10, 100, 1_000_0)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this.ByteArray = new byte[this.Size];
    }

    [Benchmark]
    public byte[] ForLoop()
    {// Slow
        for (var n = 0; n < this.ByteArray.Length; n++)
        {
            this.ByteArray[n] = 0;
        }

        return this.ByteArray;
    }

    [Benchmark]
    public byte[] ArrayFill()
    {// Fast
        Array.Fill<byte>(this.ByteArray, 0);
        return this.ByteArray;
    }

    [Benchmark]
    public byte[] SpanFill()
    {// Fast
        this.ByteArray.AsSpan().Fill(0);
        return this.ByteArray;
    }
}
