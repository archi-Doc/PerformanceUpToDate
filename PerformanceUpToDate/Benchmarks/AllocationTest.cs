// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class AllocationTest
{
    private const int Size = 32;

    public AllocationTest()
    {
    }

    [Benchmark]
    public byte[] NewByte()
    {
        return new byte[Size];
    }

    [Benchmark]
    public nint GlobalAlloc()
    {
        var p = Marshal.AllocHGlobal(Size);
        Marshal.FreeHGlobal(p);
        return p;
    }

    [Benchmark]
    public unsafe void NativeAlloc()
    {
        void* p = NativeMemory.Alloc(Size);
        NativeMemory.Free(p);
    }
}
