// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public struct SizeOfStruct
{
    public int X;
    public double Y;
    public ulong Z;
}

[Config(typeof(BenchmarkConfig))]
public class SizeOfBenchmark
{
    public SizeOfBenchmark()
    {
    }

    [Benchmark]
    public int SizeOfInt_Unsafe()
        => Unsafe.SizeOf<int>(); // 0ns

    [Benchmark]
    public int SizeOfStruct_Unsafe()
        => Unsafe.SizeOf<SizeOfStruct>(); // 0ns

    [Benchmark]
    public int SizeOfInt_Marshal()
        => Marshal.SizeOf<int>(); // 9ns

    [Benchmark]
    public int SizeOfStruct_Marshal()
        => Marshal.SizeOf<SizeOfStruct>(); // 9ns
}
